// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

public abstract class ReferencePredicate : ICompileTimeSerializable
{
    [NonCompileTimeSerialized]
    protected internal ReferencePredicateBuilder? Builder { get; }

    protected ReferencePredicate( ReferencePredicateBuilder? builder )
    {
        this.Builder = builder;
    }

    public abstract bool IsMatch( in ReferenceValidationContext context );
}