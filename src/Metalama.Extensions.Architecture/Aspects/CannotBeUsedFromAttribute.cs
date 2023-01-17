// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

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
        var currentNamespace = builder.Target.GetClosestNamedType()!.Namespace;

        if ( !this.ValidateAndProcessProperties( builder ) )
        {
            return;
        }

        var validator = new CannotBeUsedFromValidator( new MatchingRule( this ), currentNamespace.FullName );

        builder.Outbound.ValidateReferences( validator );
    }
}