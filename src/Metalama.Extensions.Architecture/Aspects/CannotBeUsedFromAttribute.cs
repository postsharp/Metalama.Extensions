// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// Aspect that, when applied to a type or member, reports a warning whenever the target type or member is used from
/// from any type specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
public class CannotBeUsedFromAttribute : BaseUsageValidationAttribute, IAspect<IMemberOrNamedType>
{
    public void BuildEligibility( IEligibilityBuilder<IMemberOrNamedType> builder ) { }

    public void BuildAspect( IAspectBuilder<IMemberOrNamedType> builder )
    {
        var predicateBuilder = new ReferencePredicateBuilder( ReferenceDirection.Outbound, builder );

        if ( !this.TryCreatePredicate( builder, predicateBuilder, out var predicate )
             || !this.TryCreateExclusionPredicate( builder, predicateBuilder, out var exclusionPredicate ) )
        {
            return;
        }

        builder.Outbound.ValidateOutboundReferences(
            new ReferencePredicateValidator(
                predicate.Not().Or( exclusionPredicate ),
                this.Description,
                this.ReferenceKinds ) );
    }
}