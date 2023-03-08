// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;
using System.Linq;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingTypePredicate : ReferencePredicate
{
    private readonly string _ns;
    private readonly string _typeName;
    private readonly ReferencingTypePredicate[]? _typeArgumentPredicates;

    public ReferencingTypePredicate( Type type, ReferencePredicateBuilder? builder = null )
        : this(
              type.Namespace ?? "",
              type.Name,

              // IsGenericType property needs to be checked first to avoid NotImplementedException from IsConstructedGenericType property getter.
              type.IsGenericType && type.IsConstructedGenericType ? type.GenericTypeArguments.Select(a => new ReferencingTypePredicate(a, builder)).ToArray() : null,
              builder ) { }

    public ReferencingTypePredicate( string ns, string typeName, ReferencingTypePredicate[]? typeArgumentPredicates = null, ReferencePredicateBuilder? builder = null ) : base( builder )
    {
        this._ns = ns;
        this._typeName = typeName;
        this._typeArgumentPredicates = typeArgumentPredicates;
    }

    private bool IsMatch(INamedType type)
    {
        if ( type.Namespace.FullName != this._ns )
        {
            return false;
        }

        if ( type.GetMetadataName() != this._typeName )
        {
            return false;
        }

        if ( type.IsGeneric && !type.IsCanonicalGenericInstance )
        {
            if (this._typeArgumentPredicates == null)
            {
                return false;
            }

            return type.TypeArguments.Select( ( a, i ) => a is INamedType n && this._typeArgumentPredicates[i].IsMatch( n ) ).All( b => b );
        }
        else
        {
            if ( this._typeArgumentPredicates != null )
            {
                return false;
            }
        }

        return true;
    }

    public override bool IsMatch( in ReferenceValidationContext context ) => this.IsMatch( context.ReferencingType );
}