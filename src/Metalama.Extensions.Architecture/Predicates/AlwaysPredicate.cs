// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class AlwaysPredicate : ReferencePredicate
{
    public AlwaysPredicate( ReferencePredicateBuilder? builder = null ) : base( builder ) { }

    public override bool IsMatch( ReferenceValidationContext context ) => true;

    public override ReferenceGranularity Granularity => ReferenceGranularity.Compilation;
}