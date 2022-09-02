// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.DependencyInjection.Implementation;
using System;

namespace Metalama.Framework.DependencyInjection.ServiceLocator;

/// <summary>
/// An implementation a dependency injection framework adapter that pulls dependency from a global <see cref="IServiceProvider"/>
/// exposed on the <see cref="ServiceProviderProvider"/> class.
/// </summary>
[CompileTime]
public class ServiceLocatorDependencyInjectionFramework : DefaultDependencyInjectionFramework
{
    protected override DefaultDependencyInjectionStrategy GetStrategy( DependencyContext context )
        => context.DependencyAttribute.GetIsLazy().GetValueOrDefault( context.Project.DependencyInjectionOptions().IsLazyByDefault )
            ? new LazyServiceLocatorDependencyInjectionStrategy( context )
            : new EarlyServiceLocatorDependencyInjectionStrategy( context );
}