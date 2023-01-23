// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class OrPredicate : ReferencePredicate
{
    private readonly ReferencePredicate _predicate1;
    private readonly ReferencePredicate _predicate2;

    public OrPredicate( ReferencePredicate predicate1, ReferencePredicate predicate2 ) : base( predicate1.Builder )
    {
        this._predicate1 = predicate1;
        this._predicate2 = predicate2;
    }

    public override bool IsMatch( in ReferenceValidationContext context )
    {
        if ( this._predicate1.IsMatch( context ) )
        {
            return true;
        }

        if ( this._predicate2.IsMatch( context ) )
        {
            return true;
        }

        return false;
    }
}