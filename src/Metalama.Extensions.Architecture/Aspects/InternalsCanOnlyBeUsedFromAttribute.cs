// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System.Linq;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// Aspect that, when applied to a public type, reports a warning whenever any member of the target type is used from
/// from a different type than the ones specified by the <see cref="BaseUsageValidationAttribute.Types"/>, <see cref="BaseUsageValidationAttribute.Namespaces"/>,
/// <see cref="BaseUsageValidationAttribute.NamespaceOfTypes"/> or <see cref="BaseUsageValidationAttribute.CurrentNamespace"/> properties.
/// </summary>
[PublicAPI]
public class InternalsCanOnlyBeUsedFromAttribute : InternalsUsageValidationAttribute
{
    protected override ReferencePredicateValidator CreateValidator( ReferencePredicate predicate )
    {
        return new ReferencePredicateValidator(
            new OrPredicate( new HasFamilyAccessPredicate(), predicate ),
            this.Description,
            this.ReferenceKinds );
    }

    public void BuildEligibility( IEligibilityBuilder<INamedType> builder ) => builder.MustHaveAccessibility( Accessibility.Public );
}

public abstract class InternalsUsageValidationAttribute : BaseUsageValidationAttribute, IAspect<INamedType>
{
    public void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        if ( !this.TryCreatePredicate( builder, out var predicate ) )
        {
            return;
        }

        var validator = this.CreateValidator( predicate );

        // Register a validator for all internal members.
        builder.Outbound.SelectMany(
                t => t.Members().Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
            .ValidateReferences( validator );

        // Also register internal accessors of public properties.
        builder.Outbound.SelectMany( t => t.Properties.Where( p => p.Accessibility is Accessibility.Public or Accessibility.Protected ) )
            .SelectMany(
                p => p.Accessors.Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
            .ValidateReferences( validator );
    }

    protected abstract ReferencePredicateValidator CreateValidator( ReferencePredicate predicate );

    public void BuildEligibility( IEligibilityBuilder<INamedType> builder ) => builder.MustHaveAccessibility( Accessibility.Public );
}