// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.DependencyInjection.Implementation;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using System;

namespace Metalama.Extensions.DependencyInjection.ServiceLocator;

#pragma warning disable IDE0079
#pragma warning disable CS8604 // Warnings that appear only on the build server.

internal class LazyServiceLocatorDependencyInjectionStrategy : DefaultDependencyInjectionStrategy, ITemplateProvider
{
    public LazyServiceLocatorDependencyInjectionStrategy( DependencyContext context ) : base( context ) { }

    public override void IntroduceDependency( IAspectBuilder<INamedType> builder )
    {
        var propertyArgs = new PropertyArgs();

        var aspectFieldOrProperty = this.Context.FieldOrProperty;

        // Introduce the visible property, something like `IMyService MyService => this._myServiceCache ??= (T) this._serviceProvider.GetService((typeof(T))`.
        var introducePropertyResult = builder.Advice.WithTemplateProvider( this )
            .IntroduceProperty(
                builder.Target,
                aspectFieldOrProperty.Name,
                nameof(GetDependencyTemplate),
                null,
                IntroductionScope.Instance,
                OverrideStrategy.Ignore,
                propertyBuilder =>
                {
                    propertyBuilder.Type = aspectFieldOrProperty.Type;
                    propertyBuilder.Name = aspectFieldOrProperty.Name;
                },
                args: new { args = propertyArgs, T = aspectFieldOrProperty.Type } );

        if ( introducePropertyResult.Outcome != AdviceOutcome.Default )
        {
            // The introduction has been ignored.
            return;
        }

        AddFields( builder, introducePropertyResult.Declaration, propertyArgs );

        this.InitializeServiceProvider( builder, propertyArgs.ServiceProviderField );
    }

    public override void ImplementDependency( IAspectBuilder<IFieldOrProperty> builder )
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
            return;
        }

        var typeBuilder = builder.WithTarget( builder.Target.DeclaringType );
        AddFields( typeBuilder, overrideResult.Declaration, propertyArgs );

        this.InitializeServiceProvider( typeBuilder, propertyArgs.ServiceProviderField );
    }

    private static void AddFields( IAspectBuilder<INamedType> builder, IProperty property, PropertyArgs propertyArgs )
    {
        // Introduce a field that stores the IServiceProvider.

        var introduceServiceProviderFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            "_serviceProvider",
            typeof(IServiceProvider) );

        if ( introduceServiceProviderFieldResult.Outcome == AdviceOutcome.Error )
        {
            return;
        }

        propertyArgs.ServiceProviderField = introduceServiceProviderFieldResult.Declaration;

        // Introduce a field that caches
        var introduceCacheFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            property.Name + "Cache",
            property.Type.ToNullableType() );

        if ( introduceCacheFieldResult.Outcome == AdviceOutcome.Error )
        {
            return;
        }

        propertyArgs.CacheField = introduceCacheFieldResult.Declaration;
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

    [Template] // Bug: Cannot be private!
    public static T GetDependencyTemplate<[CompileTime] T>( PropertyArgs args )
    {
        return args.CacheField.ToExpression().Value ??= (T) args.ServiceProviderField!.ToExpression().Value!.GetService( typeof(T) );
    }

    [Template]
    public void SetDependencyTemplate<[CompileTime] T>( PropertyArgs args )
    {
        throw new NotSupportedException( $"Cannot set '{this.Context.FieldOrProperty.Name}' because of the dependency aspect." );
    }

    [Template]
    public static void Initializer( IField serviceProviderField )
    {
        serviceProviderField.ToExpression().Value = ServiceProviderProvider.ServiceProvider();
    }
}