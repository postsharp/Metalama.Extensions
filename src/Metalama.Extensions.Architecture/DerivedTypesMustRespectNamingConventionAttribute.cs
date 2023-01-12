// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// Aspect that, when applied to a type, reports a warning for any derived type that does not respect a naming convention
/// given as a star pattern, i.e. a string where the <c>*</c> character matches any sequence of characters.
/// </summary>
[PublicAPI]
[CompileTime]
public class DerivedTypesMustRespectNamingConventionAttribute : DerivedTypesMustRespectRegexNamingConventionAttribute
{
    public DerivedTypesMustRespectNamingConventionAttribute( string pattern ) : base( ToRegex( pattern ) ) { }

    private static string ToRegex( string pattern )
    {
        const string regexPrefix = "regex:";

        if ( pattern.StartsWith( regexPrefix, StringComparison.InvariantCulture ) )
        {
            // For backward compatibility with PostSharp, we detect the prefix.
            return pattern.Substring( regexPrefix.Length );
        }

        return "^" + Regex.Escape( pattern ).Replace( "\\*", ".*" ) + "$";
    }
}