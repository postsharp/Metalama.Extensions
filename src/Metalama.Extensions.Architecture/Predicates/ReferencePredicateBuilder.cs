// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;

namespace Metalama.Extensions.Architecture.Predicates;

/// <summary>
/// An object that allows to instantiate <see cref="ReferencePredicate"/> with a fluent API.
/// Standard extension methods are provided on <see cref="ReferencePredicateExtensions"/>.
/// </summary>
[CompileTime]
[PublicAPI]
public sealed class ReferencePredicateBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicateBuilder"/> class by specifying an <see cref="ArchitectureVerifier{T}"/>.
    /// </summary>
    /// <param name="verifier">The parent <see cref="ArchitectureVerifier{T}"/>.</param>
    public ReferencePredicateBuilder( ArchitectureVerifier verifier )
    {
        this.Namespace = verifier.Namespace;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicateBuilder"/> class.
    /// </summary>
    /// <param name="ns">The namespace of the current context, used to resolve methods like <see cref="ReferencePredicateExtensions.CurrentNamespace"/>.</para>
    public ReferencePredicateBuilder( string? ns )
    {
        this.Namespace = ns;
    }

    public string? Namespace { get; }
}