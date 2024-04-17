// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
public class ReferencePredicateValidator : OutboundReferenceValidator
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
    public override void ValidateReferences( ReferenceValidationContext context )
    {
        if ( !this._allowedScope.IsMatch( context ) )
        {
            var referencedDeclaration = context.Referenced.Declaration;
            var referencedNamedType = referencedDeclaration.GetClosestNamedType();
            var referencingDeclaration = context.Referencing.Declaration;

            var optionalSpace = string.IsNullOrEmpty( this._description ) ? "" : " ";

            // Report the error message.
            context.Diagnostics.Report(
                r =>
                {
                    // Do not validate inside the same type.
                    if ( referencedNamedType != null && r.ReferencingDeclaration.IsContainedIn( referencedNamedType ) )
                    {
                        return null;
                    }
                    
                    // Allow the predicate to skip a reference instance.
                    if ( !this._allowedScope.IsMatch( r ) )
                    {
                        return null;
                    }
                    
                    // Return the error message.
                    var usageKind = (r.ReferenceKinds & ReferenceKinds.Assignment) == ReferenceKinds.Assignment ? "assigned" : "referenced";

                    

                    return ArchitectureDiagnosticDefinitions.OnlyAccessibleFrom.WithArguments(
                        (referencedDeclaration,
                         referencedDeclaration.DeclarationKind,
                         usageKind,
                         referencingDeclaration,
                         referencingDeclaration.DeclarationKind,
                         optionalSpace,
                         this._description) );
                } );
        }
    }

    public override ReferenceGranularity Granularity => this._allowedScope.Granularity;
}