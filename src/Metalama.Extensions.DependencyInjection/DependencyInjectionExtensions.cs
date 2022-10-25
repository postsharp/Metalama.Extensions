// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Project;

namespace Metalama.Extensions.DependencyInjection;

/// <summary>
/// Extends the <see cref="IProject"/> class by exposing the options that influence the handling of <see cref="IntroduceDependencyAttribute"/>.
/// </summary>
[CompileTime]
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Exposes the options that influence the handling of <see cref="IntroduceDependencyAttribute"/>.
    /// </summary>
    public static DependencyInjectionOptions DependencyInjectionOptions( this IProject project ) => project.Extension<DependencyInjectionOptions>();
}