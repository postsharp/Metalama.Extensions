// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using System;
using System.Diagnostics.CodeAnalysis;

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
    /// Initializes a new instance of the <see cref="ReferencePredicateBuilder"/> class by specifying an <see cref="TypeSetVerifier{T}"/>.
    /// </summary>
    /// <param name="verifier">The parent <see cref="TypeSetVerifier{T}"/>.</param>
    public ReferencePredicateBuilder( IVerifier<IDeclaration> verifier )
    {
        this.Namespace = verifier.Namespace;
        this.AssemblyName = verifier.AssemblyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicateBuilder"/> class.
    /// </summary>
    /// <param name="ns">The namespace of the current context, used to resolve methods like <see cref="ReferencePredicateExtensions.CurrentNamespace"/>.</param>
    /// <param name="assemblyName">The name of the current assembly, used to resolve methods like <see cref="ReferencePredicateExtensions.CurrentAssembly"/>.</param>
    public ReferencePredicateBuilder( string? ns, string? assemblyName )
    {
        this.Namespace = ns;
        this.AssemblyName = assemblyName;
    }

    /// <summary>
    /// Gets the namespace from which the current <see cref="ReferencePredicateBuilder"/> was instantiated, i.e. the namespace of the
    /// <see cref="NamespaceFabric"/> or the <see cref="TypeFabric"/>. Returns <c>null</c> if the <see cref="ReferencePredicateBuilder"/> was instantiated
    /// from a <see cref="ProjectFabric"/>. 
    /// </summary>
    public string? Namespace { get; }

    /// <summary>
    /// Gets assembly name the project that instantiated the current <see cref="IVerifier{T}"/>.
    /// </summary>
    public string? AssemblyName { get; }

    [return: NotNullIfNotNull( nameof(func) )]
    internal static ReferencePredicate? Build( Func<ReferencePredicateBuilder, ReferencePredicate>? func, IVerifier<IDeclaration> verifier )
    {
        if ( func == null )
        {
            return null;
        }
        else
        {
            return func( new ReferencePredicateBuilder( verifier ) );
        }
    }
}