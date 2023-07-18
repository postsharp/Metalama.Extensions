// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

namespace Metalama.Extensions.DependencyInjection.Implementation;

/// <summary>
/// A dependency implementation strategy that resolves the dependencies the first time they are used and pull a <see cref="Func{TResult}"/>
/// from the constructor.
/// </summary>
public partial class LazyDependencyInjectionStrategy : DefaultDependencyInjectionStrategy, ITemplateProvider
{
    public LazyDependencyInjectionStrategy( DependencyContext context ) : base( context ) { }

    public override void IntroduceDependency( IAspectBuilder<INamedType> builder )
    {
        var propertyArgs = new TemplateArgs();

        var aspectFieldOrProperty = this.Context.FieldOrProperty;

        // Introduce the visible property, something like `IMyService MyService => this._myServiceCache ??= this._myServiceFunc`.
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
                args: new { args = propertyArgs } );

        if ( introducePropertyResult.Outcome != AdviceOutcome.Default )
        {
            // The introduction has been ignored.
            return;
        }

        this.AddFields( builder, introducePropertyResult.Declaration, propertyArgs );
    }

    private void AddFields( IAspectBuilder<INamedType> builder, IProperty property, TemplateArgs propertyArgs )
    {
        // Introduce a field that stores the Func<>
        var dependencyFieldType = ((INamedType) TypeFactory.GetType( typeof(Func<>) )).WithTypeArguments( property.Type );

        var introduceFuncFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            property.Name + "Func",
            dependencyFieldType );

        if ( introduceFuncFieldResult.Outcome == AdviceOutcome.Error )
        {
            return;
        }

        propertyArgs.DependencyField = introduceFuncFieldResult.Declaration;

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

        var pullStrategy = new PullStrategy( this.Context, property, introduceFuncFieldResult.Declaration );

        this.PullDependency( builder, pullStrategy );
    }

    public override void ImplementDependency( IAspectBuilder<IFieldOrProperty> builder )
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
            return;
        }

        this.AddFields( builder.WithTarget( builder.Target.DeclaringType ), overrideResult.Declaration, templateArgs );
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
        => throw new NotSupportedException( $"Cannot set '{this.Context.FieldOrProperty.Name}' because of the dependency aspect." );
}