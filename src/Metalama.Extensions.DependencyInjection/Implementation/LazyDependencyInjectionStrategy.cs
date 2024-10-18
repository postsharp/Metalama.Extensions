// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Extensions.DependencyInjection.Implementation;

/// <summary>
/// A dependency implementation strategy that resolves the dependencies the first time they are used and pull a <see cref="Func{TResult}"/>
/// from the constructor.
/// </summary>
public partial class LazyDependencyInjectionStrategy : DefaultDependencyInjectionStrategy, ITemplateProvider
{
    public LazyDependencyInjectionStrategy( DependencyProperties properties ) : base( properties ) { }

    public override bool TryIntroduceDependency( IAspectBuilder<INamedType> builder, [NotNullWhen( true )] out IFieldOrProperty? fieldOrProperty )
    {
        var propertyArgs = new TemplateArgs();

        // Introduce the visible property, something like `IMyService MyService => this._myServiceCache ??= this._myServiceFunc`.
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
                args: new { args = propertyArgs } );

        if ( introducePropertyResult.Outcome != AdviceOutcome.Default )
        {
            // The introduction has been ignored.
            fieldOrProperty = introducePropertyResult.Declaration;

            return true;
        }

        this.TryAddFields( builder, introducePropertyResult.Declaration, propertyArgs );

        fieldOrProperty = introducePropertyResult.Declaration;

        return true;
    }

    private bool TryAddFields( IAspectBuilder<INamedType> builder, IProperty property, TemplateArgs propertyArgs )
    {
        // Introduce a field that stores the Func<>
        var dependencyFieldType = ((INamedType) TypeFactory.GetType( typeof(Func<>) )).WithTypeArguments( property.Type );

        var introduceFuncFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            property.Name + "Func",
            dependencyFieldType );

        if ( introduceFuncFieldResult.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        SuppressionHelper.SuppressUnusedWarnings( builder, introduceFuncFieldResult.Declaration );

        propertyArgs.DependencyField = introduceFuncFieldResult.Declaration;

        // Introduce a field that caches
        var introduceCacheFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            property.Name + "Cache",
            property.Type.ToNullable() );

        if ( introduceCacheFieldResult.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        SuppressionHelper.SuppressUnusedWarnings( builder, introduceCacheFieldResult.Declaration );

        propertyArgs.CacheField = introduceCacheFieldResult.Declaration;

        var pullStrategy = new PullStrategy( this.Properties, property, introduceFuncFieldResult.Declaration );

        return this.TryPullDependency( builder, propertyArgs.DependencyField, pullStrategy );
    }

    public override bool TryImplementDependency( IAspectBuilder<IFieldOrProperty> builder )
    {
        var templateArgs = new TemplateArgs();

        var overrideResult = builder.Advice
            .WithTemplateProvider( this )
            .OverrideAccessors(
                builder.Target,
                nameof(GetDependencyTemplate),
                builder.Target.Writeability != Writeability.None ? nameof(this.SetDependencyTemplate) : null,
                args: new { args = templateArgs } );

        if ( overrideResult.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        SuppressNonNullableFieldMustContainValue( builder, builder.Target );

        return this.TryAddFields( builder.With( builder.Target.DeclaringType ), overrideResult.Declaration, templateArgs );
    }

    public class TemplateArgs
    {
        public IField? CacheField { get; set; }

        public IField? DependencyField { get; set; }
    }

    [Template]
    private static dynamic? GetDependencyTemplate( TemplateArgs args ) => args.CacheField!.Value ??= args.DependencyField!.Value!.Invoke();

    // ReSharper disable once UnusedParameter.Local
    [Template]
    private void SetDependencyTemplate( TemplateArgs args )
        => throw new NotSupportedException( $"Cannot set '{this.Properties.Name}' because of the dependency aspect." );
}