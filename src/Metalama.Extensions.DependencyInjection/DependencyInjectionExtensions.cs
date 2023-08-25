// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.DependencyInjection.Implementation;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Extensions.DependencyInjection;

/// <summary>
/// Extends the <see cref="IProject"/> and <see cref="IAspectBuilder"/> interfaces.
/// </summary>
[CompileTime]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Exposes the options that influence the handling of <see cref="IntroduceDependencyAttribute"/>.
    /// </summary>
    public static DependencyInjectionOptions DependencyInjectionOptions( this IProject project ) => project.Extension<DependencyInjectionOptions>();

    /// <summary>
    /// Tries to introduce a dependency into a specified type. 
    /// </summary>
    /// <param name="aspectBuilder">An <see cref="IAspectBuilder"/>.</param>
    /// <param name="properties">The properties of the dependency to introduce.</param>
    /// <param name="dependencyFieldOrProperty">When the method succeeds, the field or dependency that represents the dependency.</param>
    /// <returns><c>true</c> in case of success, otherwise <c>false</c>. When the field or property already exists, this method returns <c>true</c> and has no effect.</returns>
    public static bool TryIntroduceDependency(
        this IAspectBuilder aspectBuilder,
        DependencyProperties properties,
        [NotNullWhen( true )] out IFieldOrProperty? dependencyFieldOrProperty )
    {
        if ( !aspectBuilder.Project.DependencyInjectionOptions().TryGetFramework( properties, aspectBuilder.Diagnostics, out var framework ) )
        {
            aspectBuilder.SkipAspect();

            dependencyFieldOrProperty = null;

            return false;
        }

        return framework.TryIntroduceDependency( properties, aspectBuilder.WithTarget( properties.TargetType ), out dependencyFieldOrProperty );
    }
}