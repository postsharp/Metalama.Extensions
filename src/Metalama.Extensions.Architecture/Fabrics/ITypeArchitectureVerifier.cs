// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Extensions.Architecture.Fabrics;

/// <summary>
/// Exposes, as extension methods, the fluent API for architecture validation from fabrics. Standard extension methods are exposed by the <see cref="ArchitectureVerifierExtensions"/> class.
/// To extend the fluent API, expose extension methods of this object.
/// </summary>
/// <typeparam name="T">Either <see cref="ICompilation"/>, <see cref="INamespace"/> or <see cref="INamespace"/>.</typeparam>
[CompileTime]
public interface ITypeArchitectureVerifier<out T> : IArchitectureVerifier<T>
    where T : class, IDeclaration
{
    /// <summary>
    /// Gets an <see cref="IAspectReceiver{TDeclaration}"/> that selects all types in the current scope.
    /// </summary>
    IAspectReceiver<INamedType> TypeReceiver { get; }
}