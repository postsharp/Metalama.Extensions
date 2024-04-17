// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class HasFamilyAccessPredicate : ReferencePredicate
{
    public HasFamilyAccessPredicate( ReferencePredicateBuilder? builder = null ) : base( builder ) { }

    public override bool IsMatch( ReferenceValidationContext context )
    {
        // TODO: take nested types into account.
        return context.Referenced.Declaration is IMember { Accessibility: Accessibility.Protected or Accessibility.ProtectedInternal }
               && context.Referencing.Type.Is( context.Referenced.Declaration.GetClosestNamedType()! );
    }

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;
}