// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Validators;

internal class InternalsCanOnlyBeUsedFromValidator : CanOnlyBeUsedFromValidator
{
    public InternalsCanOnlyBeUsedFromValidator( MatchingRule rule, string? currentNamespace ) : base(
        rule,
        currentNamespace ) { }

    public override void Validate( in ReferenceValidationContext context )
    {
        if ( !InternalsHelper.HasFamilyAccess( context ) )
        {
            base.Validate( in context );
        }
    }

    public override string ConstraintName => "InternalsCanOnlyBeUsedFrom";
}