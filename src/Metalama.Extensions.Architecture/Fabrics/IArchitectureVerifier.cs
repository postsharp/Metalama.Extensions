// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.Fabrics;

/// <summary>
/// Exposes, as extension methods, the fluent API for architecture validation from fabrics. When the scope represents a set of types,
/// the <see cref="ITypeArchitectureVerifier{T}"/> interface is used instead.
/// </summary>
[CompileTime]
[PublicAPI]
public interface IArchitectureVerifier<out T>
    where T : class, IDeclaration
{
    /// <summary>
    /// Gets the namespace from which the current <see cref="IArchitectureVerifier{T}"/> was instantiated, i.e. the namespace of the
    /// <see cref="NamespaceFabric"/> or the <see cref="TypeFabric"/>. Returns <c>null</c> if the <see cref="IArchitectureVerifier{T}"/> was instantiated
    /// from a <see cref="ProjectFabric"/>. 
    /// </summary>
    string? Namespace { get; }

    /// <summary>
    /// Gets the underlying <see cref="IAspectReceiver{TDeclaration}"/>.
    /// </summary>
    IAspectReceiver<T> Receiver { get; }
}