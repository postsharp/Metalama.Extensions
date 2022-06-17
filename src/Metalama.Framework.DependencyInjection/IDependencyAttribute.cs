// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Framework.DependencyInjection;

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