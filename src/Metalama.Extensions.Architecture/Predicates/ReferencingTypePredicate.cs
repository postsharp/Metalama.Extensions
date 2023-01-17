// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Validation;
using System;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingTypePredicate : ReferencePredicate
{
    private readonly string _ns;
    private readonly string _typeName;

    public ReferencingTypePredicate( Type type, ReferencePredicateBuilder? builder = null ) : this( type.Namespace ?? "", type.Name, builder ) { }

    public ReferencingTypePredicate( string ns, string typeName, ReferencePredicateBuilder? builder = null ) : base( builder )
    {
        this._ns = ns;
        this._typeName = typeName;
    }

    public override bool IsMatch( in ReferenceValidationContext context )
        => context.ReferencingType.Name == this._typeName && context.ReferencingType.Namespace.FullName == this._ns;
}