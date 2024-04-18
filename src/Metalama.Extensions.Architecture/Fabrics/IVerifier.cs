// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using System;

namespace Metalama.Extensions.Architecture.Fabrics;

/// <summary>
/// Exposes, as extension methods, the fluent API for architecture validation from fabrics. When the scope represents a set of types,
/// the <see cref="ITypeSetVerifier{T}"/> interface is used instead. This interface represents a set of declarations that can be validated.
/// </summary>
[CompileTime]
[PublicAPI]
[Obsolete( "Use IAspectReceiver<T>." )]
public interface IVerifier<out T>
    where T : class, IDeclaration
{
    /// <summary>
    /// Gets assembly name the project that instantiated the current <see cref="IVerifier{T}"/>.
    /// </summary>
    string AssemblyName { get; }

    /// <summary>
    /// Gets the namespace from which the current <see cref="IVerifier{T}"/> was instantiated, i.e. the namespace of the
    /// <see cref="NamespaceFabric"/> or the <see cref="TypeFabric"/>. Returns <c>null</c> if the <see cref="IVerifier{T}"/> was instantiated
    /// from a <see cref="ProjectFabric"/>. 
    /// </summary>
    string? Namespace { get; }

    /// <summary>
    /// Gets the underlying <see cref="IAspectReceiver{TDeclaration}"/>.
    /// </summary>
    IAspectReceiver<T> Receiver { get; }
}