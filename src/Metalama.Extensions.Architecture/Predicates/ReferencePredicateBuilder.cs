// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
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
    [Obsolete]
    public ReferencePredicateBuilder( IVerifier<IDeclaration> verifier ) : this( ReferenceEndRole.Origin, verifier.Namespace, verifier.AssemblyName ) { }

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
        this.Context = new ReferencePredicateBuilderContext( validatedRole, ns, assemblyName );
    }

    internal ReferencePredicateBuilder( ReferencePredicateBuilder parent, PredicateModifier modifier )
    {
        this.Context = parent.Context;
        this.Modifier = modifier;
    }

    public ReferencePredicateBuilderContext Context { get; }

    [Obsolete( "Use the Context property." )]
    public ReferenceEndRole ValidatedRole => this.Context.ValidatedRole;

    [Obsolete( "Use the Context property." )]
    public string? Namespace => this.Context.Namespace;

    [Obsolete( "Use the Context property." )]
    public string? AssemblyName => this.Context.AssemblyName;

    internal PredicateModifier? Modifier { get; }

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