// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class NotPredicate : ReferencePredicate
{
    private readonly ReferencePredicate _predicate;

    public NotPredicate( ReferencePredicate predicate ) : base( predicate.Builder )
    {
        this._predicate = predicate;
    }

    protected override bool IsMatchCore( ReferenceValidationContext context ) => !this._predicate.IsMatch( context );

    public override ReferenceGranularity Granularity => this._predicate.Granularity;
}