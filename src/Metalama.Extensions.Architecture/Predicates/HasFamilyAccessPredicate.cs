// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class HasFamilyAccessPredicate : ReferencePredicate
{
    private readonly ReferenceDirection _direction;

    public HasFamilyAccessPredicate( ReferencePredicateBuilder builder ) : base( builder )
    {
        this._direction = builder.Direction;
    }

    public override bool IsMatch( ReferenceValidationContext context )
    {
        ref var b = ref context.GetReferenceEnd( this._direction );
        ref var a = ref context.GetReferenceEnd( this._direction == ReferenceDirection.Inbound ? ReferenceDirection.Outbound : ReferenceDirection.Inbound );

        return a.Declaration is IMemberOrNamedType { Accessibility: Accessibility.Protected or Accessibility.ProtectedInternal }
               && b.Type.Is( a.Member.GetClosestNamedType()! );
    }

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;
}