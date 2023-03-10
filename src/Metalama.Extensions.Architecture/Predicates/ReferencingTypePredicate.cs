// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingTypePredicate : ReferencePredicate
{
    private readonly string _ns;
    private readonly string _typeName;

    // IsGenericType property needs to be checked first to avoid NotImplementedException from IsConstructedGenericType property getter.
    public ReferencingTypePredicate( Type type, ReferencePredicateBuilder? builder = null )
        : base( builder )
    {
        if ( type.IsGenericType && !type.IsGenericTypeDefinition )
        {
            throw new InvalidOperationException( $"The '{type}' type cannot be used as a referencing type predicate parameter. Bound generic types are not allowed." );
        }

        this._ns = type.Namespace ?? "";
        this._typeName = GetName( type );
    }

    private static string GetName( Type type )
            => type.DeclaringType == null
            ? type.Name
            : $"{GetName( type.DeclaringType )}+{type.Name}";

    private static string GetName( INamedType type )
        => type.DeclaringType == null
        ? type.GetMetadataName()
        : $"{GetName( type.DeclaringType )}+{type.GetMetadataName()}";

    private bool IsMatch( INamedType type )
        => type.Namespace.FullName == this._ns && GetName( type ) == this._typeName;

    public override bool IsMatch( in ReferenceValidationContext context )
        => this.IsMatch( context.ReferencingType );
}