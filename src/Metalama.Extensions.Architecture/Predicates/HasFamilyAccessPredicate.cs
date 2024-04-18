// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class HasFamilyAccessPredicate : ReferencePredicate
{
    private readonly ReferenceEndRole _role;

    public HasFamilyAccessPredicate( ReferencePredicateBuilder builder ) : base( builder )
    {
        this._role = builder.ValidatedRole;
    }

    public override bool IsMatch( ReferenceValidationContext context )
    {
        // TODO: take nested types into account.

        ref var b = ref context.GetReferenceEnd( this._role );
        ref var a = ref context.GetReferenceEnd( this._role == ReferenceEndRole.Destination ? ReferenceEndRole.Origin : ReferenceEndRole.Destination );

        return a.Declaration is IMemberOrNamedType { Accessibility: Accessibility.Protected or Accessibility.ProtectedInternal }
               && b.Type.Is( a.Member.GetClosestNamedType()! );
    }

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;
}