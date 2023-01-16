// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Validators;

internal class InternalsCanOnlyBeUsedFromValidator : CanOnlyBeUsedFromValidator
{
    public InternalsCanOnlyBeUsedFromValidator( UsageRule rule, string? currentNamespace ) : base(
        rule,
        currentNamespace ) { }

    public override void Validate( in ReferenceValidationContext context )
    {
        // Do not validate if we have visibility through inheritance.
        // TODO: take nested types into account.
        if ( context.ReferencingType.Is( context.ReferencedDeclaration.GetClosestNamedType()! ) )
        {
            return;
        }

        base.Validate( in context );
    }

    public override string ConstraintName => "InternalsCanOnlyBeUsedFrom";
}