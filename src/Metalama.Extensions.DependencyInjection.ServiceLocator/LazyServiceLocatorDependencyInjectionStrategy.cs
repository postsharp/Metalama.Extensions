// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.DependencyInjection.Implementation;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Extensions.DependencyInjection.ServiceLocator;

#pragma warning disable IDE0079
#pragma warning disable CS8604 // Warnings that appear only on the build server.

internal class LazyServiceLocatorDependencyInjectionStrategy : DefaultDependencyInjectionStrategy, ITemplateProvider
{
    public LazyServiceLocatorDependencyInjectionStrategy( DependencyProperties properties ) : base( properties ) { }

    public override bool TryIntroduceDependency( IAspectBuilder<INamedType> builder, [NotNullWhen( true )] out IFieldOrProperty? fieldOrProperty )
    {
        var propertyArgs = new PropertyArgs();

        // Introduce the visible property, something like `IMyService MyService => this._myServiceCache ??= (T) this._serviceProvider.GetService((typeof(T))`.
        var introducePropertyResult = builder.Advice.WithTemplateProvider( this )
            .IntroduceProperty(
                builder.Target,
                this.Properties.Name,
                nameof(GetDependencyTemplate),
                null,
                IntroductionScope.Instance,
                OverrideStrategy.Ignore,
                propertyBuilder =>
                {
                    propertyBuilder.Type = this.Properties.DependencyType;
                    propertyBuilder.Name = this.Properties.Name;
                },
                args: new { args = propertyArgs, T = this.Properties.DependencyType } );

        fieldOrProperty = introducePropertyResult.Declaration;

        if ( introducePropertyResult.Outcome != AdviceOutcome.Default )
        {
            // The introduction has been ignored.
            return true;
        }

        if ( !this.TryAddFields( builder, introducePropertyResult.Declaration, propertyArgs ) )
        {
            return false;
        }

        this.InitializeServiceProvider( builder, propertyArgs.ServiceProviderField );

        return true;
    }

    public override bool TryImplementDependency( IAspectBuilder<IFieldOrProperty> builder )
    {
        var propertyArgs = new PropertyArgs();

        var overrideResult = builder.Advice
            .WithTemplateProvider( this )
            .OverrideAccessors(
                builder.Target,
                nameof(GetDependencyTemplate),
                builder.Target.Writeability != Writeability.None ? nameof(this.SetDependencyTemplate) : null,
                args: new { args = propertyArgs, T = builder.Target.Type } );

        if ( overrideResult.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        var typeBuilder = builder.WithTarget( builder.Target.DeclaringType );

        if ( !this.TryAddFields( typeBuilder, overrideResult.Declaration, propertyArgs ) )
        {
            return false;
        }

        return true;
    }

    private bool TryAddFields( IAspectBuilder<INamedType> builder, IProperty property, PropertyArgs propertyArgs )
    {
        // Introduce a field that stores the IServiceProvider.

        var introduceServiceProviderFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            "_serviceProvider",
            typeof(IServiceProvider),
            whenExists: OverrideStrategy.Ignore );

        if ( introduceServiceProviderFieldResult.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        propertyArgs.ServiceProviderField = introduceServiceProviderFieldResult.Declaration;

        if ( introduceServiceProviderFieldResult.Outcome != AdviceOutcome.Ignore )
        {
            this.InitializeServiceProvider( builder, propertyArgs.ServiceProviderField );
        }

        // Introduce a field that caches the service.
        var introduceCacheFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            property.Name + "Cache",
            property.Type.ToNullableType() );

        if ( introduceCacheFieldResult.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        propertyArgs.CacheField = introduceCacheFieldResult.Declaration;

        return true;
    }

    private void InitializeServiceProvider( IAspectBuilder<INamedType> builder, IField serviceProviderField )
    {
        foreach ( var constructor in builder.Target.Constructors )
        {
            if ( constructor.InitializerKind != ConstructorInitializerKind.This )
            {
                builder.Advice.WithTemplateProvider( this )
                    .AddInitializer( constructor, nameof(Initializer), args: new { serviceProviderField } );
            }
        }
    }

    public class PropertyArgs
    {
        public IField? CacheField { get; set; }

        public IField? ServiceProviderField { get; set; }
    }

    [Template]
    private static T GetDependencyTemplate<[CompileTime] T>( PropertyArgs args )
    {
        return args.CacheField!.Value ??= (T) args.ServiceProviderField!.Value!.GetService( typeof(T) );
    }

    // ReSharper disable once UnusedParameter.Local
    [Template]
    private void SetDependencyTemplate<[CompileTime] T>( PropertyArgs args )
    {
        throw new NotSupportedException( $"Cannot set '{this.Properties.Name}' because of the dependency aspect." );
    }

    [Template]
    private static void Initializer( IField serviceProviderField )
    {
        serviceProviderField.Value = ServiceProviderProvider.ServiceProvider();
    }
}