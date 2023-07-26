// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// Aspect that, when applied to a type, reports a warning for any derived type that does not respect a naming convention
/// given as a star pattern, i.e. a string where the <c>*</c> character matches any sequence of characters.
/// </summary>
[PublicAPI]
[CompileTime]
public class DerivedTypesMustRespectNamingConventionAttribute : DerivedTypesMustRespectRegexNamingConventionAttribute
{
    private readonly string _starPattern;

    public DerivedTypesMustRespectNamingConventionAttribute( string pattern ) : base( NamingConventionHelper.StarPatternToRegex( pattern ), pattern )
    {
        this._starPattern = pattern;
    }

    public override string ToString() => $"DerivedTypesMustRespectNamingConvention( \"{this._starPattern}\" )";
}