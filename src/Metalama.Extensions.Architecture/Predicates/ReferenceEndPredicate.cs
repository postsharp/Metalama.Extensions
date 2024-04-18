// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;
using System;
using System.Diagnostics;

namespace Metalama.Extensions.Architecture.Predicates;

public abstract class ReferenceEndPredicate : ReferencePredicate
{
    public ReferenceDirection Direction { get; }

    protected ReferenceEndPredicate( ReferencePredicateBuilder builder ) : base( builder )
    {
        this.Direction = builder.Direction;
    }

    public override bool IsMatch( ReferenceValidationContext context )
    {
        return this.IsMatch( context.GetReferenceEnd( this.Direction ) );
    }

    public abstract bool IsMatch( in ReferenceEnd referenceEnd );
}