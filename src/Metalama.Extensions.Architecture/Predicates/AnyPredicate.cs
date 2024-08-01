﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;
using System.Collections.Immutable;

namespace Metalama.Extensions.Architecture.Predicates;

internal class AnyPredicate : ReferencePredicate
{
    private ImmutableArray<ReferencePredicate> _predicates;

    public AnyPredicate( ImmutableArray<ReferencePredicate> predicates, ReferencePredicateBuilder builder ) : base( builder )
    {
        this._predicates = predicates;
    }

    protected override bool IsMatchCore( ReferenceValidationContext context )
    {
        foreach ( var predicate in this._predicates )
        {
            if ( predicate.IsMatch( context ) )
            {
                return true;
            }
        }

        return false;
    }

    protected override ReferenceGranularity GetGranularity()
    {
        var granularity = ReferenceGranularity.Compilation;

        foreach ( var predicate in this._predicates )
        {
            granularity = granularity.CombineWith( predicate.Granularity );
        }

        return granularity;
    }
}