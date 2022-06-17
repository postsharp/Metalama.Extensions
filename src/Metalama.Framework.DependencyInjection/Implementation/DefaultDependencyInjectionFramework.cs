// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Framework.DependencyInjection.Implementation;

public class DefaultDependencyInjectionFramework : IDependencyInjectionFramework
{
    public virtual bool CanHandleDependency( DependencyContext context ) => true;

    public void IntroduceDependency( IntroduceDependencyContext context, IAspectBuilder<INamedType> aspectBuilder )
    {
        this.GetStrategy( context ).IntroduceDependency( aspectBuilder );
    }

    public void ImplementDependency( ImplementDependencyContext context, IAspectBuilder<IFieldOrProperty> aspectBuilder )
    {
        this.GetStrategy( context ).ImplementDependency( aspectBuilder );
    }

    protected virtual DefaultDependencyInjectionStrategy GetStrategy( DependencyContext context )
        => !context.DependencyAttribute.GetIsLazy().GetValueOrDefault( context.Project.DependencyInjectionOptions().IsLazyByDefault )
            ? new DefaultDependencyInjectionStrategy( context )
            : new LazyDependencyInjectionStrategy( context );
}