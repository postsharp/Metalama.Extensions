// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// Aspect that, when applied to a type or member, reports a warning whenever the target type or member is used from
/// from any type specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
public class CannotBeUsedFromAttribute : BaseUsageValidationAttribute, IAspect<IMemberOrNamedType>
{
    public void BuildEligibility( IEligibilityBuilder<IMemberOrNamedType> builder ) { }

    private protected override bool IsMatch( in ReferenceValidationContext context ) => !base.IsMatch( in context );

    public void BuildAspect( IAspectBuilder<IMemberOrNamedType> builder )
    {
        if ( !this.ValidateAndProcessProperties( builder, builder.Target.GetClosestNamedType()!.Namespace ) )
        {
            return;
        }

        builder.With( x => x ).ValidateReferences( this.ValidateReference, ReferenceKinds.All );
    }
}