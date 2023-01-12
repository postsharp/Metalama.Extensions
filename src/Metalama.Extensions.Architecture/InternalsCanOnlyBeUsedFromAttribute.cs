// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Validation;
using System.Linq;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// Aspect that, when applied to a public type, reports a warning whenever any member of the target type is used from
/// from a different type than the ones specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
[PublicAPI]
public class InternalsCanOnlyBeUsedFromAttribute : BaseUsageValidationAttribute, IAspect<INamedType>
{
    public void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        if ( !this.ValidateAndProcessProperties( builder, builder.Target.Namespace ) )
        {
            return;
        }

        // Register a validator for all internal members.
        builder.With(
                t => t.Members().Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
            .ValidateReferences( this.ValidateReference, ReferenceKinds.All );
    }

    protected override void ValidateReference( in ReferenceValidationContext context )
    {
        // Do not validate if we have visibility through inheritance.
        // TODO: take nested types into account.
        if ( ((IMember) context.ReferencedDeclaration).Accessibility is Accessibility.ProtectedInternal &&
             context.ReferencingType.Is( context.ReferencedDeclaration.GetClosestNamedType()! ) )
        {
            return;
        }

        base.ValidateReference( context );
    }

    public void BuildEligibility( IEligibilityBuilder<INamedType> builder ) => builder.MustHaveAccessibility( Accessibility.Public );
}