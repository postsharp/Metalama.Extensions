// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Framework.DependencyInjection.Implementation;

/// <summary>
/// Interface that dependency injection framework adapters must implement to handle the <see cref="DependencyAttribute"/> advice.
/// An implementation typically also implements <see cref="IPullStrategy"/>.
/// </summary>
[CompileTime]
public interface IDependencyInjectionFramework
{
    /// <summary>
    /// Determines whether the current instance can weave a given aspect dependency advice into a given type. The implementation can
    /// report diagnostics to <see cref="DependencyInjectionContext.Diagnostics"/>.
    /// </summary>
    bool CanInjectDependency( DependencyInjectionContext context );

    /// <summary>
    /// Injects the dependency into the target type.
    /// </summary>
    /// <param name="context">Information regarding the dependency to inject.</param>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target type.</param>
    void InjectDependency( DependencyInjectionContext context, IAspectBuilder<INamedType> aspectBuilder );
}