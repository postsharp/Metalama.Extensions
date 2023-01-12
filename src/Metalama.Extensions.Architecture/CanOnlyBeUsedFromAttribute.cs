// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// Aspect that, when applied to a type or member, reports a warning whenever the target type or member is used from a different type
/// than the ones specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
[PublicAPI]
public class CanOnlyBeUsedFromAttribute : BaseUsageValidationAttribute, IAspect<IMemberOrNamedType>
{
    public void BuildEligibility( IEligibilityBuilder<IMemberOrNamedType> builder ) { }

    public void BuildAspect( IAspectBuilder<IMemberOrNamedType> builder )
    {
        if ( !this.ValidateAndProcessProperties( builder, builder.Target.GetClosestNamedType()!.Namespace ) )
        {
            return;
        }

        builder.With( x => x ).ValidateReferences( this.ValidateReference, ReferenceKinds.All );
    }
}