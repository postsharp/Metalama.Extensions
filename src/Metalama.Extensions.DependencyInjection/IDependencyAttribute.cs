// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Extensions.DependencyInjection;

/// <summary>
/// A common interface for <see cref="DependencyAttribute"/> and <see cref="IntroduceDependencyAttribute"/>.
/// </summary>
[RunTimeOrCompileTime]
public interface IDependencyAttribute
{
    /// <summary>
    /// Gets the value of the <see cref="IntroduceDependencyAttribute.IsLazy"/> if it has been assigned, or <c>null</c> if it has not been assigned.
    /// </summary>
    bool? GetIsLazy();

    /// <summary>
    /// Gets the value of the <see cref="IntroduceDependencyAttribute.IsRequired"/> if it has been assigned, or <c>null</c> if it has not been assigned.
    /// </summary>
    bool? GetIsRequired();
}