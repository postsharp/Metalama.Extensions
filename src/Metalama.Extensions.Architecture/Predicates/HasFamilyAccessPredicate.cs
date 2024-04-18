// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class HasFamilyAccessPredicate : ReferenceEndPredicate
{
    public HasFamilyAccessPredicate( ReferencePredicateBuilder builder ) : base( builder ) { }

    public override bool IsMatch( in ReferenceEnd referenceEnd )
    {
        // TODO: take nested types into account.
        return referenceEnd.Declaration is IMember { Accessibility: Accessibility.Protected or Accessibility.ProtectedInternal }
               && referenceEnd.Type.Is( referenceEnd.Declaration.GetClosestNamedType()! );
    }

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;
}