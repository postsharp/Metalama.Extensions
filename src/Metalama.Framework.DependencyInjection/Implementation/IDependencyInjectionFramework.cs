// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Framework.DependencyInjection.Implementation;

/// <summary>
/// Interface that dependency injection framework adapters must implement to handle the <see cref="IntroduceDependencyAttribute"/> advice.
/// An implementation typically also implements <see cref="IPullStrategy"/>.
/// </summary>
[CompileTime]
public interface IDependencyInjectionFramework
{
    /// <summary>
    /// Determines whether the current instance can handle a <see cref="DependencyAttribute"/> aspect or <see cref="IntroduceDependencyAttribute"/> advice.
    /// The implementation can report diagnostics to <see cref="IntroduceDependencyContext.Diagnostics"/>.
    /// </summary>
    /// <param name="context">A <see cref="IntroduceDependencyContext"/> or <see cref="ImplementDependencyContext"/>.</param>
    bool CanHandleDependency( DependencyContext context );

    /// <summary>
    /// Injects the dependency into the target type.
    /// </summary>
    /// <param name="context">Information regarding the dependency to inject.</param>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target type.</param>
    void IntroduceDependency( IntroduceDependencyContext context, IAspectBuilder<INamedType> aspectBuilder );

    void ImplementDependency( ImplementDependencyContext context, IAspectBuilder<IFieldOrProperty> aspectBuilder );
}