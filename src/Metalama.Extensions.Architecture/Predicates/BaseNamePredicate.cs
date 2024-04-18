// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Predicates;

[CompileTime]
internal abstract class BaseNamePredicate : ReferenceEndPredicate
{
    private const string _identifierChar = "[\\p{L}\\p{M}\\p{N}\\p{Pc}\\p{Cf}]";
    private const string _starPattern = _identifierChar + "*";
    private const string _dotStarStarDotPattern = "(\\." + _identifierChar + "+)*\\.";
    private const string _starStarDotPattern = "(" + _identifierChar + "+\\.)*";
    private const string _dotStarStarPattern = "(\\." + _identifierChar + "+)*";

    private readonly string _name;
    private readonly bool _isRegex;

    [NonCompileTimeSerialized]
    private Regex? _regex;

    protected BaseNamePredicate( string name, ReferencePredicateBuilder builder ) : base( builder )
    {
        if ( name.Contains( "*" ) )
        {
            this._name = GetRegexPattern( name );
            this._isRegex = true;
        }
        else
        {
            this._name = name;
        }
    }

    protected bool IsMatch( string name )
    {
        if ( !this._isRegex )
        {
            return name == this._name;
        }
        else
        {
            this._regex ??= new Regex( this._name );

            return this._regex.IsMatch( name );
        }
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