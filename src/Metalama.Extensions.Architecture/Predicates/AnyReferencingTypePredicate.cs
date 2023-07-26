// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metalama.Extensions.Architecture.Predicates;

internal class AnyReferencingTypePredicate : ReferencePredicate
{
    private readonly IRef<IDeclaration>[] _typeRefs;

    public AnyReferencingTypePredicate( IEnumerable<Type> types, ReferencePredicateBuilder? builder = null )
        : base( builder )
    {
        var iTypes = types.Select( TypeFactory.GetType );
        var typeRefs = new List<IRef<IDeclaration>>();

        foreach ( var iType in iTypes )
        {
            if ( iType is not INamedType namedType )
            {
                throw new InvalidOperationException(
                    $"The type '{iType}' cannot be used as a referencing type predicate parameter. Arrays, type parameters and pointers are not allowed." );
            }

            if ( namedType is { IsGeneric: true, IsCanonicalGenericInstance: false } )
            {
                throw new InvalidOperationException(
                    $"The type '{iType}' cannot be used as a referencing type predicate parameter. Bound generic types are not allowed." );
            }

            typeRefs.Add( namedType.GetOriginalDefinition().ToRef() );
        }

        this._typeRefs = typeRefs.ToArray();
    }

    public override bool IsMatch( in ReferenceValidationContext context )
    {
        var contextCopy = context;

        return this._typeRefs.Any( t => contextCopy.ReferencingType.Equals( t.GetTarget( options: default ) ) );
    }
}