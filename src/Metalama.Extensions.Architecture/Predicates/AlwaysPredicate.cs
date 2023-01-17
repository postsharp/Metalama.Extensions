// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class AlwaysPredicate : ReferencePredicate
{
    public AlwaysPredicate( ReferencePredicateBuilder? builder = null ) : base( builder ) { }

    public override bool IsMatch( in ReferenceValidationContext context ) => true;
}