// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;
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
    /// Gets the role of the <see cref="ReferenceEnd"/> validated by the built predicates.
    /// </summary>
    public ReferenceEndRole ValidatedRole { get; }

    [Obsolete]
    public ReferencePredicateBuilder( IVerifier<IDeclaration> verifier )
    {
        this.Namespace = verifier.Namespace;
        this.AssemblyName = verifier.AssemblyName;
        this.ValidatedRole = ReferenceEndRole.Origin;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicateBuilder"/> class from an <see cref="IAspectReceiver{TDeclaration}"/>.
    /// </summary>
    public ReferencePredicateBuilder( ReferenceEndRole validatedRole, IAspectReceiver<IDeclaration> receiver ) : this(
        validatedRole,
        receiver.OriginatingNamespace,
        receiver.Project.AssemblyName ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicateBuilder"/> class from an <see cref="IAspectBuilder{TAspectTarget}"/>.
    /// </summary>
    public ReferencePredicateBuilder( ReferenceEndRole validatedRole, IAspectBuilder<IDeclaration> aspectBuilder ) : this(
        validatedRole,
        aspectBuilder.Target.GetNamespace()?.FullName,
        aspectBuilder.Project.AssemblyName ) { }

    private ReferencePredicateBuilder( ReferenceEndRole validatedRole, string? ns = null, string? assemblyName = null )
    {
        this.ValidatedRole = validatedRole;
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
    internal static ReferencePredicate? Build(
        Func<ReferencePredicateBuilder, ReferencePredicate>? func,
        IAspectReceiver<IDeclaration> verifier,
        ReferenceEndRole role )
    {
        if ( func == null )
        {
            return null;
        }
        else
        {
            return func( new ReferencePredicateBuilder( role, verifier ) );
        }
    }
}