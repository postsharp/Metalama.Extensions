// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Predicates;

internal class ReferencingNamespacePredicate : ReferencePredicate
{
    private const string _identifierChar = "\\p{L}";
    private const string _starPattern = _identifierChar + "*";
    private const string _dotStarStarDotPattern = "(\\." + _identifierChar + "+)*\\.";
    private const string _starStarDotPattern = "(" + _identifierChar + "+\\.)*";
    private const string _dotStarStarPattern = "(\\." + _identifierChar + "+)*";

    private readonly string _ns;
    private readonly bool _isRegex;

    [NonCompileTimeSerialized]
    private Regex? _regex;

    public ReferencingNamespacePredicate( string ns, ReferencePredicateBuilder? builder = null ) : base( builder )
    {
        if ( ns.Contains( "*" ) )
        {
            this._ns = GetRegexPattern( ns );
            this._isRegex = true;
        }
        else
        {
            this._ns = ns;
        }
    }

    public override bool IsMatch( in ReferenceValidationContext context )
    {
        for ( var ns = context.ReferencingType.Namespace; ns != null; ns = ns.ParentNamespace )
        {
            if ( !this._isRegex )
            {
                if ( ns.FullName == this._ns )
                {
                    return true;
                }
            }
            else
            {
                this._regex ??= new Regex( this._ns );

                if ( this._regex.IsMatch( ns.FullName ) )
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string GetRegexPattern( string starPattern )
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append( "^" );

        for ( var i = 0; i < starPattern.Length; i++ )
        {
            var c = starPattern[i];

            if ( c == '.' )
            {
                if ( starPattern.Length == i + 3 && starPattern[i + 1] == '*' && starPattern[i + 2] == '*' )
                {
                    stringBuilder.Append( _dotStarStarPattern );

                    break;
                }
                else if ( starPattern.Length >= i + 4 && starPattern[i + 1] == '*' && starPattern[i + 2] == '*' && starPattern[i + 3] == '.' )
                {
                    stringBuilder.Append( _dotStarStarDotPattern );
                    i += 3;
                }
                else
                {
                    stringBuilder.Append( "\\." );
                }
            }
            else if ( c == '*' )
            {
                if ( starPattern.Length >= i + 3 && starPattern[i + 1] == '*' && starPattern[i + 2] == '.' )
                {
                    stringBuilder.Append( _starStarDotPattern );
                    i += 2;
                }
                else
                {
                    stringBuilder.Append( _starPattern );
                }
            }
            else
            {
                stringBuilder.Append( c );
            }
        }

        stringBuilder.Append( "$" );

        return stringBuilder.ToString();
    }
}