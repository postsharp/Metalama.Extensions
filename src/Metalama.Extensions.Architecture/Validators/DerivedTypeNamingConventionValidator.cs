// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
internal class DerivedTypeNamingConventionValidator : OutboundReferenceValidator
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
        var regex = this.GetRegex();

        if ( regex.IsMatch( context.Referencing.Type.Name ) )
        {
            return false;
        }

        if ( this._exclusion != null && this._exclusion.IsMatch( context ) )
        {
            return false;
        }

        return true;
    }

    public override void ValidateReferences( ReferenceValidationContext context )
    {
        if ( this.IsMatch( context ) )
        {
            context.Diagnostics.Report(
                x => x.ReferenceKinds == ReferenceKinds.BaseType
                    ? ArchitectureDiagnosticDefinitions.NamingConventionViolationInDerivedType.WithArguments(
                        (context.Referencing.Type,
                         context.Referenced.Type, this._displayPattern) )
                    : null,
                x => x.ReferencingDeclaration );
        }
    }

    public override bool IncludeDerivedTypes => true;

    public override ReferenceGranularity Granularity => ReferenceGranularity.Type;

    public override ReferenceKinds ValidatedReferenceKinds => ReferenceKinds.BaseType;
}