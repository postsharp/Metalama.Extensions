﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
public class ReferencePredicateValidator : ReferenceValidator
{
    private readonly ReferencePredicate _allowedScope;
    private readonly string? _description;

    public override ReferenceKinds ValidatedReferenceKinds { get; }

    public ReferencePredicateValidator( ReferencePredicate allowedScope, string? description, ReferenceKinds validatedReferenceKinds )
    {
        this._allowedScope = allowedScope;
        this._description = description;
        this.ValidatedReferenceKinds = validatedReferenceKinds;
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

        if ( !this._allowedScope.IsMatch( context ) )
        {
            var optionalSpace = string.IsNullOrEmpty( this._description ) ? "" : " ";

            string usageKind;

            if ( (context.ReferenceKinds & ReferenceKinds.Assignment) == ReferenceKinds.Assignment )
            {
                usageKind = "assigned";
            }
            else
            {
                usageKind = "referenced";
            }

            // Report the error message.
            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.OnlyAccessibleFrom.WithArguments(
                    (context.ReferencedDeclaration,
                     context.ReferencedDeclaration.DeclarationKind,
                     usageKind,
                     context.ReferencingType,
                     optionalSpace,
                     this._description) ) );
        }
    }
}