// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.DependencyInjection.Implementation;
using System;

namespace Metalama.Framework.DependencyInjection.DotNet;

internal class LazyDependencyInjectionStrategy : DefaultDependencyInjectionStrategy, ITemplateProvider
{
    public LazyDependencyInjectionStrategy( DependencyInjectionContext context ) : base( context ) { }

    public override void Implement( IAspectBuilder<INamedType> builder )
    {
        var propertyArgs = new PropertyTags();

        var aspectFieldOrProperty = this.Context.AspectFieldOrProperty!;

        // Introduce the visible property, something like `IMyService MyService => this._myServiceCache ??= this._myServiceFunc`.
        var introducePropertyResult = builder.Advice.WithTemplateProvider( this )
            .IntroduceProperty(
                builder.Target,
                nameof(this.PropertyTemplate),
                IntroductionScope.Instance,
                OverrideStrategy.Ignore,
                propertyBuilder =>
                {
                    propertyBuilder.Type = aspectFieldOrProperty.Type;
                    propertyBuilder.Name = aspectFieldOrProperty.Name;
                },
                tags: propertyArgs );

        if ( introducePropertyResult.Outcome != AdviceOutcome.Default )
        {
            // The introduction has been ignored.
            return;
        }

        // Introduce a field that stores the Func<>
        var dependencyFieldType = ((INamedType) TypeFactory.GetType( typeof(Func<>) )).ConstructGenericInstance( aspectFieldOrProperty.Type );

        var introduceFuncFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            aspectFieldOrProperty!.Name + "Func",
            dependencyFieldType );

        if ( introduceFuncFieldResult.Outcome == AdviceOutcome.Error )
        {
            return;
        }

        propertyArgs.DependencyField = introduceFuncFieldResult.Declaration;

        // Introduce a field that caches
        var introduceCacheFieldResult = builder.Advice.IntroduceField(
            builder.Target,
            aspectFieldOrProperty.Name + "Cache",
            aspectFieldOrProperty.Type.ConstructNullable() );

        if ( introduceCacheFieldResult.Outcome == AdviceOutcome.Error )
        {
            return;
        }

        propertyArgs.CacheField = introduceCacheFieldResult.Declaration;

        var pullStrategy = new LazyPullStrategy( this.Context, introducePropertyResult.Declaration, introduceFuncFieldResult.Declaration );

        this.PullDependency( builder, pullStrategy );
    }

    private class PropertyTags
    {
        public IField? CacheField { get; set; }

        public IField? DependencyField { get; set; }
    }

    [Template]
    public dynamic PropertyTemplate
    {
        get
        {
            var tags = (PropertyTags) meta.Tags.Source!;

            return ExpressionFactory.Parse( $"(this.{tags.CacheField!.Name} ??= {tags.DependencyField!.Name}())" ).Value!;
        }
    }
}