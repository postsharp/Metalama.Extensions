// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Extensions.DependencyInjection.Implementation;

/// <summary>
/// Interface that dependency injection framework adapters must implement to handle the <see cref="IntroduceDependencyAttribute"/> advice.
/// An implementation typically also implements <see cref="IPullStrategy"/>.
/// </summary>
[CompileTime]
public interface IDependencyInjectionFramework
{
    /// <summary>
    /// Determines whether the current instance can handle a <see cref="DependencyAttribute"/> aspect or <see cref="IntroduceDependencyAttribute"/> advice.
    /// </summary>
    /// <param name="properties"> <see cref="DependencyProperties"/> or <see cref="DependencyProperties"/>.</param>
    /// <param name="diagnostics">A <see cref="ScopedDiagnosticSink"/>.</param>
    bool CanHandleDependency( DependencyProperties properties, in ScopedDiagnosticSink diagnostics );

    /// <summary>
    /// Processes the <see cref="IntroduceDependencyAttribute"/> advice, i.e. introduce a dependency defined by a custom aspect into the target
    /// type of the aspect.
    /// </summary>
    /// <param name="properties">Information regarding the dependency to inject.</param>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder{TAspectTarget}"/> for the target type.</param>
    /// <param name="dependencyFieldOrProperty"></param>
    bool TryIntroduceDependency( DependencyProperties properties, IAspectBuilder<INamedType> aspectBuilder, out IFieldOrProperty? dependencyFieldOrProperty );

    /// <summary>
    /// Processes the <see cref="DependencyAttribute"/> aspect, i.e. changes the target field or property of the aspect into a dependency. 
    /// </summary>
    /// <param name="properties">Information regarding the dependency to inject.</param>
    /// <param name="aspectBuilder">The <see cref="IAspectBuilder{TAspectTarget}"/> for the field or property to pull.</param>
    bool TryImplementDependency( DependencyProperties properties, IAspectBuilder<IFieldOrProperty> aspectBuilder );
}