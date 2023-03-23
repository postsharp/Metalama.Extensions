// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

namespace Metalama.Extensions.Architecture.Fabrics;

/// <summary>
/// Exposes, as extension methods, the fluent API for architecture validation from fabrics. Standard extension methods are exposed by the <see cref="VerifierExtensions"/> class.
/// To extend the fluent API, expose extension methods of this object. This interface is an <see cref="IVerifier{T}"/> for declarations that contain types, i.e.
/// <see cref="ICompilation"/>, <see cref="INamespace"/> or <see cref="INamedType"/>. The types in the set can be accessed with the <see cref="VerifierExtensions.Types"/> extension method.
/// </summary>
[CompileTime]
[PublicAPI]
public interface ITypeSetVerifier<out T> : IVerifier<T>
    where T : class, IDeclaration
{
    /// <summary>
    /// Gets an <see cref="IAspectReceiver{TDeclaration}"/> that selects all types in the current scope.
    /// </summary>
    IAspectReceiver<INamedType> TypeReceiver { get; }

    /// <summary>
    /// Selects the types in the current set that are derived from a given type.
    /// </summary>
    ITypeSetVerifier<INamedType> SelectTypesDerivedFrom( Type type, DerivedTypesOptions options = DerivedTypesOptions.Default );
}