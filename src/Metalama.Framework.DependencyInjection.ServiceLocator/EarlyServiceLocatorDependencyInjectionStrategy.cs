// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.DependencyInjection.Implementation;
using System;

namespace Metalama.Framework.DependencyInjection.ServiceLocator;

internal class EarlyServiceLocatorDependencyInjectionStrategy : DefaultDependencyInjectionStrategy, ITemplateProvider
{
    public EarlyServiceLocatorDependencyInjectionStrategy( DependencyContext context ) : base( context ) { }

    protected override void PullDependency( IAspectBuilder<INamedType> aspectBuilder, IPullStrategy pullStrategy, IConstructor constructor )
    {
        aspectBuilder.Advice.WithTemplateProvider( this )
            .AddInitializer(
                constructor,
                nameof(this.InitializerTemplate),
                args: new { T = this.Context.FieldOrProperty.Type, fieldOrProperty = this.Context.FieldOrProperty } );
    }

    [Template]
    public void InitializerTemplate<T>( IFieldOrProperty fieldOrProperty )
    {
        var isRequired = this.Context.DependencyAttribute.GetIsRequired()
            .GetValueOrDefault( this.Context.Project.DependencyInjectionOptions().IsRequiredByDefault );

        if ( isRequired )
        {
            fieldOrProperty.ToExpression().Value =
                (T) ServiceProviderProvider.ServiceProvider().GetService( typeof(T) )
                ?? throw new InvalidOperationException( $"The service '{fieldOrProperty.Type}' could not be obtained from the service locator." );
        }
        else
        {
            fieldOrProperty.ToExpression().Value =
                (T) ServiceProviderProvider.ServiceProvider().GetService( typeof(T) );
        }
    }
}