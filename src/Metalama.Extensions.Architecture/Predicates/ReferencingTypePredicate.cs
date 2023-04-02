// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;

namespace Metalama.Extensions.Architecture.Predicates;

internal partial class ReferencingTypePredicate : ReferencePredicate
{
    private readonly IRef<IDeclaration> _typeRef;

    public ReferencingTypePredicate( Type type, ReferencePredicateBuilder? builder = null )
        : base( builder )
    {
        var iType = TypeFactory.GetType( type );

        if ( iType is not INamedType namedType )
        {
            throw new InvalidOperationException(
                $"The type '{type}' cannot be used as a referencing type predicate parameter. Arrays, type parameters and pointers are not allowed." );
        }

        if ( namedType is { IsGeneric: true, IsCanonicalGenericInstance: false } )
        {
            throw new InvalidOperationException(
                $"The type '{type}' cannot be used as a referencing type predicate parameter. Bound generic types are not allowed." );
        }

        this._typeRef = namedType.GetOriginalDefinition().ToRef();
    }

    public override bool IsMatch( in ReferenceValidationContext context ) => context.ReferencingType.Equals( this._typeRef.GetTarget( options: default ) );
}