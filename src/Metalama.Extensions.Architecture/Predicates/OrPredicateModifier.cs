// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class OrPredicateModifier : PredicateModifier
{
    public ReferencePredicate OtherPredicate { get; }

    public OrPredicateModifier( ReferencePredicate otherPredicate )
    {
        this.OtherPredicate = otherPredicate;
    }

    public override bool IsMatch( bool currentPredicateResult, ReferenceValidationContext context )
        => currentPredicateResult || this.OtherPredicate.IsMatch( context );
}