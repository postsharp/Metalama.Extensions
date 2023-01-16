// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Aspects;

[CompileTime]
internal abstract partial class BaseUsageValidator : ReferenceValidator
{
    private const string _identifierChar = "\\p{L}";
    private const string _starPattern = _identifierChar + "*";
    private const string _dotStarStarDotPattern = "(\\." + _identifierChar + "+)*\\.";
    private const string _starStarDotPattern = "(" + _identifierChar + "+\\.)*";
    private const string _dotStarStarPattern = "(\\." + _identifierChar + "+)*";

    private readonly ImmutableDictionary<string, NamespaceData> _namespaces;
    private ImmutableArray<string> _namespacePatterns;

    protected BaseUsageValidator( UsageRule rule, string? currentNamespace )
    {
        var namespacesBuilder = ImmutableDictionary.CreateBuilder<string, NamespaceData>();
        var namespacePatternsBuilder = ImmutableArray.CreateBuilder<string>();

        // First process all namespaces.
        foreach ( var ns in rule.AllowedNamespaces )
        {
            if ( ns.Contains( "*" ) )
            {
                // This is a pattern.
                var pattern = GetRegexPattern( ns );
                namespacePatternsBuilder.Add( pattern );
            }
            else
            {
                namespacesBuilder[ns] = new NamespaceData();
            }
        }

        foreach ( var type in rule.AllowedNamespaceOfTypes )
        {
            namespacesBuilder[type.Namespace ?? ""] = new NamespaceData();
        }

        if ( rule.AllowCurrentNamespace && currentNamespace != null )
        {
            namespacesBuilder[currentNamespace] = new NamespaceData();
        }

        // Process types.
        foreach ( var type in rule.AllowedTypes )
        {
            var ns = type.Namespace ?? "";

            if ( namespacesBuilder.TryGetValue( ns, out var namespaceData ) )
            {
                if ( namespaceData.AnyType )
                {
                    // The new type specification is redundant.
                    continue;
                }
            }
            else
            {
                namespaceData = new NamespaceData() { AnyType = false };
            }

            namespaceData = new NamespaceData() { AnyType = false, SpecificTypes = namespaceData.SpecificTypes.Add( type.Name ) };

            namespacesBuilder[ns] = namespaceData;
        }

        this._namespaces = namespacesBuilder.ToImmutable();
        this._namespacePatterns = namespacePatternsBuilder.ToImmutable();
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

    /// <summary>
    /// Determines whether the current context matches the rule of the current aspect. When is method returns <c>false</c>,
    /// the <see cref="Validate"/> method reports an error.
    /// </summary>
    private bool IsMatch( in ReferenceValidationContext context )
    {
        for ( var ns = context.ReferencingType.Namespace; ns != null; ns = ns.ParentNamespace )
        {
            // Check exactly named namespaces.
            if ( this._namespaces.TryGetValue( ns.FullName, out var namespaceData ) )
            {
                if ( namespaceData.AnyType )
                {
                    return true;
                }
                else if ( namespaceData.SpecificTypes.Contains( context.ReferencingType.Name ) )
                {
                    return true;
                }
            }

            // Check namespace patterns.
            if ( !this._namespacePatterns.IsDefaultOrEmpty )
            {
                foreach ( var pattern in this._namespacePatterns )
                {
                    if ( Regex.IsMatch( ns.FullName, pattern ) )
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsAllowed( in ReferenceValidationContext context )
    {
        if ( this.OnMatch == MatchBehavior.Allow )
        {
            return this.IsMatch( context );
        }
        else
        {
            return !this.IsMatch( context );
        }
    }

    /// <summary>
    /// Validates a reference and reports a warning if necessary.
    /// </summary>
    public override void Validate( in ReferenceValidationContext context )
    {
        // Do not validate inside the same type.
        var closestNamedType = context.ReferencedDeclaration.GetClosestNamedType();
        if ( closestNamedType != null && context.ReferencingDeclaration.IsContainedIn( closestNamedType ) )
        {
            return;
        }

        if ( !this.IsAllowed( context ) )
        {
            // Report the error message.
            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.OnlyAccessibleFrom.WithArguments(
                    (context.ReferencedDeclaration, context.ReferencedDeclaration.DeclarationKind, context.ReferencingType, this.GetType().Name) ) );
        }
    }
    
    public abstract string ConstraintName { get; }
    
     protected abstract MatchBehavior OnMatch { get; }

    protected enum MatchBehavior
    {
        Allow,
        Forbid
    }
}