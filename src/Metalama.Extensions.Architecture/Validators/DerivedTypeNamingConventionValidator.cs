// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
internal class DerivedTypeNamingConventionValidator : ReferenceValidator
{
    private readonly string _regexPattern;
    private readonly string _displayPattern;
    private readonly ReferencePredicate? _exclusion;

    private DerivedTypeNamingConventionValidator( string regexPattern, string displayPattern, ReferencePredicate? exclusion )
    {
        this._regexPattern = regexPattern;
        this._displayPattern = displayPattern;
        this._exclusion = exclusion;
    }

    public static DerivedTypeNamingConventionValidator CreateRegexValidator( string regexPattern, string displayPattern, ReferencePredicate? exclusion = null )
        => new( regexPattern, displayPattern, exclusion );

    public static DerivedTypeNamingConventionValidator CreateRegexValidator( string regexPattern, ReferencePredicate? exclusion = null )
        => new( regexPattern, regexPattern, exclusion );

    public static DerivedTypeNamingConventionValidator CreateStarPatternValidator( string pattern, ReferencePredicate? exclusion = null )
        => new( NamingConventionHelper.StarPatternToRegex( pattern ), pattern, exclusion );

    // This field exists for performance reasons. It is not serialized, so it is
    // lazily recreated every time it is needed.
    [NonCompileTimeSerialized]
    private Regex? _regex;

    private Regex GetRegex() => this._regex ??= new Regex( this._regexPattern, RegexOptions.CultureInvariant );

    /// <summary>
    /// Returns <c>true</c> if an error message should be reported.
    /// </summary>
    private bool IsMatch( in ReferenceValidationContext context )
    {
        if ( context.ReferenceKinds != ReferenceKinds.BaseType )
        {
            // We may have a call of Validate for a type construction, e.g. IEnumerable<Foo>,
            // and we don't want to match these indirect references.
            return false;
        }

        var regex = this.GetRegex();

        var referencingType = (INamedType) context.ReferencingDeclaration;

        if ( regex.IsMatch( referencingType.Name ) )
        {
            return false;
        }

        Debugger.Break();

        if ( this._exclusion != null && this._exclusion.IsMatch( context ) )
        {
            return false;
        }

        return true;
    }

    public override void Validate( in ReferenceValidationContext context )
    {
        if ( this.IsMatch( context ) )
        {
            var referencingType = (INamedType) context.ReferencingDeclaration;

            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.NamingConventionViolationInDerivedType.WithArguments(
                    (referencingType, (INamedType) context.ReferencedDeclaration, this._displayPattern) ),
                context.ReferencingDeclaration );
        }
    }

    public override bool IncludeDerivedTypes => true;

    public override ReferenceKinds ValidatedReferenceKinds => ReferenceKinds.BaseType;
}