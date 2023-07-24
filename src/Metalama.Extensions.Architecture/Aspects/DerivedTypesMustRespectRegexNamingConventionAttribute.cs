// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// Aspect that, when applied to a type, reports a warning for any derived type that does not respect a naming convention
/// given as regular expression.
/// </summary>
[PublicAPI]
public class DerivedTypesMustRespectRegexNamingConventionAttribute : TypeAspect
{
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
            _ = new Regex( this.Pattern );
        }
        catch ( Exception e )
        {
            builder.Diagnostics.Report( ArchitectureDiagnosticDefinitions.InvalidRegex.WithArguments( (this.Pattern, e.Message) ) );
            builder.SkipAspect();

            return;
        }

        builder.Outbound.ValidateReferences( DerivedTypeNamingConventionValidator.CreateRegexValidator( this.Pattern ) );
    }
    
    public override string ToString() => $"DerivedTypesMustRespectRegexNamingConvention( \"{this.Pattern}\" )";
}