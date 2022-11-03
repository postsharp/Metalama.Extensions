// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System;

namespace Metalama.Extensions.Multicast;

// [Obsolete] cannot be an error because it is used from the compile-time compilation, which does not copy [Obsolete].

/// <summary>
///   Kind of inheritance of <see cref = "IMulticastAttribute" />.
/// </summary>
[Obsolete( "Inheritance is Metalama is implemented at the aspect class level with the [Inherited] attribute." )]
[RunTimeOrCompileTime]
public enum MulticastInheritance
{
    /// <summary>
    ///   No inheritance.
    /// </summary>
    None,

    /// <summary>
    ///   The instance is inherited to children of the original element,
    ///   but multicasting is not applied to members of children.
    /// </summary>
    /// <remarks>
    ///   See https://doc.postsharp.net/multicast-inheritance.
    /// </remarks>
    Strict,

    /// <summary>
    ///   The instance is inherited to children of the original element
    ///   and multicasting is applied to members of children.
    /// </summary>
    /// <remarks>
    ///   See https://doc.postsharp.net/multicast-inheritance.
    /// </remarks>
    Multicast
}