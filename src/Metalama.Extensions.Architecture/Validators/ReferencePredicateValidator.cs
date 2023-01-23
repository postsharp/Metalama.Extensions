// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
public class ReferencePredicateValidator : ReferenceValidator
{
    private readonly ReferencePredicate _predicate;
    private readonly string? _description;

    public ReferencePredicateValidator( ReferencePredicate predicate, string? description )
    {
        this._predicate = predicate;
        this._description = description;
    }

    /// <summary>
    /// Validates a reference and reports a warning if necessary.
    /// </summary>
    public override void Validate( in ReferenceValidationContext context )
    {
        // Do not validate inside the same type.
        var closestNamedType = context.ReferencedDeclaration.GetClosestNamedType();

        if ( closestNamedType != null && context.ReferencingDeclaration.IsContainedIn( closestNamedType ) )
        {
            return;
        }

        if ( !this._predicate.IsMatch( context ) )
        {
            var optionalSpace = string.IsNullOrEmpty( this._description ) ? "" : " ";

            // Report the error message.
            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.OnlyAccessibleFrom.WithArguments(
                    (context.ReferencedDeclaration, context.ReferencedDeclaration.DeclarationKind, context.ReferencingType, optionalSpace,
                     this._description) ) );
        }
    }
}