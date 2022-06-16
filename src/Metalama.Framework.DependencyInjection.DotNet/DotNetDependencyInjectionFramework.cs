// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.DependencyInjection.Implementation;

namespace Metalama.Framework.DependencyInjection.DotNet;

/// <summary>
/// An implementation of <see cref="IDependencyInjectionFramework"/> for the <c>Microsoft.Extensions.DependencyInjection</c>.
/// </summary>
internal class DotNetDependencyInjectionFramework : IDependencyInjectionFramework, ITemplateProvider
{
    // Microsoft.Extensions.DependencyInjection does not require any attribute, so we can handle any field or property.
    bool IDependencyInjectionFramework.CanInjectDependency( DependencyInjectionContext context ) => true;

    public void InjectDependency( DependencyInjectionContext context, IAspectBuilder<INamedType> aspectBuilder )
    {
        GetStrategy( context ).Implement( aspectBuilder );
    }

    private static DefaultDependencyInjectionStrategy GetStrategy( DependencyInjectionContext context ) 
        => !context.DependencyAttribute.GetIsLazy().GetValueOrDefault()
            ? new DefaultDependencyInjectionStrategy( context )
            : new LazyDependencyInjectionStrategy( context );
}