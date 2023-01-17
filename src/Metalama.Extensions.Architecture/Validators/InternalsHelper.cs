// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
internal static class InternalsHelper
{
    public static bool HasFamilyAccess( in ReferenceValidationContext context )
    {
        // TODO: take nested types into account.
        return context.ReferencedDeclaration is IMember { Accessibility: Accessibility.Protected or Accessibility.ProtectedInternal }
               && context.ReferencingType.Is( context.ReferencedDeclaration.GetClosestNamedType()! );
    }
}