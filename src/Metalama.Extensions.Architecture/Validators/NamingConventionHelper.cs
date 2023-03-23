// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Validators;

[CompileTime]
internal static class NamingConventionHelper
{
    public static string StarPatternToRegex( string pattern )
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