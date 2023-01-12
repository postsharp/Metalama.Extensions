// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// Aspect that, when applied to a type, reports a warning for any derived type that does not respect a naming convention
/// given as regular expression.
/// </summary>
[PublicAPI]
public class DerivedTypesMustRespectRegexNamingConventionAttribute : TypeAspect
{
    // This field exists for performance reasons. It is not serialized, so it is
    // lazily recreated every time it is needed.
    [LamaNonSerialized]
    private Regex? _regex;

    public DerivedTypesMustRespectRegexNamingConventionAttribute( string pattern )
    {
        this.Pattern = pattern;
    }

    public string Pattern { get; }

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Validate the regex.
        try
        {
            this.GetRegex();
        }
        catch ( Exception e )
        {
            builder.Diagnostics.Report( ArchitectureDiagnosticDefinitions.InvalidRegex.WithArguments( (this.Pattern, e.Message) ) );
            builder.SkipAspect();

            return;
        }

        builder.With( t => t ).ValidateReferences( this.ValidateReferences, ReferenceKinds.BaseType );
    }

    private Regex GetRegex() => this._regex ??= new Regex( this.Pattern, RegexOptions.CultureInvariant );

    private void ValidateReferences( in ReferenceValidationContext context )
    {
        var regex = this.GetRegex();

        var referencingType = (INamedType) context.ReferencingDeclaration;

        if ( !regex.IsMatch( referencingType.Name ) )
        {
            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.NamingConventionViolation.WithArguments(
                    (referencingType, (INamedType) context.ReferencedDeclaration, this.Pattern) ) );
        }
    }
}