// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

/// <summary>
/// A <see cref="ReferencePredicate"/> that relies on a single <see cref="ReferenceEnd"/> (referencing or referenced).
/// </summary>
[PublicAPI]
public abstract class ReferenceEndPredicate : ReferencePredicate
{
    /// <summary>
    /// Gets the role of the <see cref="ReferenceEnd"/> being validated.
    /// </summary>
    public ReferenceEndRole ValidatedRole { get; }

    protected ReferenceEndPredicate( ReferencePredicateBuilder builder ) : base( builder )
    {
        this.ValidatedRole = builder.ValidatedRole;
    }

    public sealed override bool IsMatch( ReferenceValidationContext context ) => this.IsMatch( context.GetReferenceEnd( this.ValidatedRole ) );

    /// <summary>
    /// Gets a value indicating whether the predicate matches the given <see cref="ReferenceEnd"/>.
    /// </summary>
    public abstract bool IsMatch( in ReferenceEnd referenceEnd );
}