// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Serialization;
using System.Collections.Immutable;
using System.Text;

namespace Metalama.Extensions.Architecture.Aspects;

[CompileTime]
internal class SerializableAccessibilityRule : IAspectState
{
    private const string _identifierChar = "\\p{L}";
    private const string _starPattern = _identifierChar + "*";
    private const string _dotStarStarDotPattern = "(\\." + _identifierChar + "+)*\\.";
    private const string _starStarDotPattern = "(" + _identifierChar + "+\\.)*";
    private const string _dotStarStarPattern = "(\\." + _identifierChar + "+)*";

    public ImmutableDictionary<string, NamespaceData> Namespaces { get; }

    public ImmutableArray<string> NamespacePatterns { get; }

    public SerializableAccessibilityRule( BaseUsageValidationAttribute attribute, INamespace currentNamespace, in ScopedDiagnosticSink diagnosticSink )
    {
        var namespacesBuilder = ImmutableDictionary.CreateBuilder<string, NamespaceData>();
        var namespacePatternsBuilder = ImmutableArray.CreateBuilder<string>();

        // First process all namespaces.
        foreach ( var ns in attribute.Namespaces )
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

        foreach ( var type in attribute.NamespaceOfTypes )
        {
            namespacesBuilder[type.Namespace] = new NamespaceData();
        }

        if ( attribute.CurrentNamespace )
        {
            namespacesBuilder[currentNamespace.FullName] = new NamespaceData();
        }

        // Process types.
        foreach ( var type in attribute.Types )
        {
            var ns = type.Namespace;

            if ( namespacesBuilder.TryGetValue( ns, out var namespaceData ) )
            {
                if ( namespaceData.AnyType )
                {
                    // The new type specification is redundant.
                    diagnosticSink.Report( ArchitectureDiagnosticDefinitions.OnlyAccessibleFromRedundantType.WithArguments( type ) );

                    continue;
                }
            }
            else
            {
                namespaceData = new NamespaceData() { AnyType = false };
            }

            namespaceData = new NamespaceData() { AnyType = false, SpecificTypes = namespaceData.SpecificTypes!.Add( type.Name ) };

            namespacesBuilder[ns] = namespaceData;
        }

        this.Namespaces = namespacesBuilder.ToImmutable();
        this.NamespacePatterns = namespacePatternsBuilder.ToImmutable();
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

    public class NamespaceData : ILamaSerializable
    {
        public bool AnyType { get; set; } = true;

        public ImmutableHashSet<string> SpecificTypes { get; set; } = ImmutableHashSet<string>.Empty;
    }
}