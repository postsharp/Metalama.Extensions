// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

namespace Metalama.Framework.DependencyInjection.Implementation;

/// <summary>
/// The default implementation of <see cref="IDependencyInjectionFramework"/>. It pulls dependencies from all constructors and use <see cref="Func{TResult}"/>
/// to accept lazy dependencies.
/// </summary>
public class DefaultDependencyInjectionFramework : IDependencyInjectionFramework
{
    /// <inheritdoc />
    public virtual bool CanHandleDependency( DependencyContext context ) => !context.FieldOrProperty.IsStatic;

    /// <inheritdoc />
    public void IntroduceDependency( IntroduceDependencyContext context, IAspectBuilder<INamedType> aspectBuilder )
    {
        this.GetStrategy( context ).IntroduceDependency( aspectBuilder );
    }

    /// <inheritdoc />
    public void ImplementDependency( ImplementDependencyContext context, IAspectBuilder<IFieldOrProperty> aspectBuilder )
    {
        this.GetStrategy( context ).ImplementDependency( aspectBuilder );
    }

    /// <summary>
    /// Gets an instance of the <see cref="DefaultDependencyInjectionStrategy"/> class for a given context.
    /// </summary>
    protected virtual DefaultDependencyInjectionStrategy GetStrategy( DependencyContext context )
        => !context.DependencyAttribute.GetIsLazy().GetValueOrDefault( context.Project.DependencyInjectionOptions().IsLazyByDefault )
            ? new DefaultDependencyInjectionStrategy( context )
            : new LazyDependencyInjectionStrategy( context );
}