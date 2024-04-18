// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingDeclarationHasFamilyAccessToReferencedTypePredicate : ReferencePredicate
{
    public ReferencingDeclarationHasFamilyAccessToReferencedTypePredicate( ReferencePredicateBuilder builder ) : base( builder ) { }

    public override bool IsMatch( ReferenceValidationContext context )
    {
        return context.Referenced.Declaration is IMemberOrNamedType { Accessibility: Accessibility.Protected or Accessibility.ProtectedInternal }
               && context.Referencing.Type.Is( context.Referenced.Member.GetClosestNamedType()! );
    }

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;
}