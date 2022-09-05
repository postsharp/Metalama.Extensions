// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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
    /// The implementation can report diagnostics to <see cref="DependencyContext.Diagnostics"/>.
    /// </summary>
    /// <param name="context">A <see cref="IntroduceDependencyContext"/> or <see cref="ImplementDependencyContext"/>.</param>
    bool CanHandleDependency( DependencyContext context );

    /// <summary>
    /// Processes the <see cref="IntroduceDependencyAttribute"/> advice, i.e. introduce a dependency defined by a custom aspect into the target
    /// type of the aspect.
    /// </summary>
    /// <param name="context">Information regarding the dependency to inject.</param>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target type.</param>
    void IntroduceDependency( IntroduceDependencyContext context, IAspectBuilder<INamedType> aspectBuilder );

    /// <summary>
    /// Processes the <see cref="DependencyAttribute"/> aspect, i.e. changes the target field or property of the aspect into a dependency. 
    /// </summary>
    /// <param name="context">Information regarding the dependency to inject.</param>
    /// <param name="aspectBuilder">The <see cref="IAspectBuilder{TAspectTarget}"/> for the field or property to pull.</param>
    void ImplementDependency( ImplementDependencyContext context, IAspectBuilder<IFieldOrProperty> aspectBuilder );
}