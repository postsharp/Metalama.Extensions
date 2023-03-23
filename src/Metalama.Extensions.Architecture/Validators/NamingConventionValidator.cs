// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
internal class NamingConventionValidator : ReferenceValidator
{
    private readonly string _pattern;

    private NamingConventionValidator( string pattern )
    {
        this._pattern = pattern;
    }

    public static NamingConventionValidator CreateRegexValidator( string pattern ) => new( pattern );

    public static NamingConventionValidator CreateStarPatternValidator( string pattern ) => new( NamingConventionHelper.StarPatternToRegex( pattern ) );

    // This field exists for performance reasons. It is not serialized, so it is
    // lazily recreated every time it is needed.
    [NonCompileTimeSerialized]
    private Regex? _regex;

    private Regex GetRegex() => this._regex ??= new Regex( this._pattern, RegexOptions.CultureInvariant );

    public override void Validate( in ReferenceValidationContext context )
    {
        var regex = this.GetRegex();

        var referencingType = (INamedType) context.ReferencingDeclaration;

        if ( !regex.IsMatch( referencingType.Name ) )
        {
            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.NamingConventionViolationInDerivedType.WithArguments(
                    (referencingType, (INamedType) context.ReferencedDeclaration, this._pattern) ) );
        }
    }

    public override ReferenceKinds ValidatedReferenceKinds => ReferenceKinds.BaseType;
}