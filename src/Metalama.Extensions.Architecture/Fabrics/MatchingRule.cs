// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Metalama.Extensions.Architecture.Fabrics
{
    [CompileTime]
    public sealed class MatchingRule
    {
        internal MatchingRule() { }

        internal bool MatchingAlways { get; init; }

        internal ImmutableArray<string> MatchingNamespaces { get; init; } = ImmutableArray<string>.Empty;

        internal ImmutableArray<Type> MatchingNamespaceOfTypes { get; init; } = ImmutableArray<Type>.Empty;

        internal ImmutableArray<Type> MatchingTypes { get; init; } = ImmutableArray<Type>.Empty;

        internal bool MatchingCurrentNamespace { get; init; }

        public static MatchingRule OwnNamespace { get; } = new() { MatchingCurrentNamespace = true };

        public static MatchingRule Never { get; } = new();

        public static MatchingRule Always { get; } = new() { MatchingAlways = true };

        public static MatchingRule Namespace( string ns ) => new() { MatchingNamespaces = ImmutableArray.Create( ns ) };

        public static MatchingRule Namespaces( IEnumerable<string> namespaces ) => new() { MatchingNamespaces = namespaces.ToImmutableArray() };

        public static MatchingRule Type( Type type ) => new() { MatchingTypes = ImmutableArray.Create( type ) };

        public static MatchingRule Types( IEnumerable<Type> types ) => new() { MatchingTypes = types.ToImmutableArray() };

        public static MatchingRule NamespaceOfType( Type type ) => new() { MatchingNamespaceOfTypes = ImmutableArray.Create( type ) };

        public static MatchingRule NamespaceOfTypes( IEnumerable<Type> types ) => new() { MatchingNamespaceOfTypes = types.ToImmutableArray() };

        internal MatchingRule( BaseUsageValidationAttribute attribute )
        {
            this.MatchingNamespaces = ImmutableArray.Create( attribute.Namespaces );
            this.MatchingCurrentNamespace = attribute.CurrentNamespace;
            this.MatchingTypes = ImmutableArray.Create( attribute.Types );
            this.MatchingNamespaceOfTypes = ImmutableArray.Create( attribute.NamespaceOfTypes );
        }

        public MatchingRule Or( MatchingRule rule )
            => new()
            {
                MatchingTypes = this.MatchingTypes.AddRange( rule.MatchingTypes ),
                MatchingNamespaces = this.MatchingNamespaces.AddRange( rule.MatchingNamespaces ),
                MatchingNamespaceOfTypes = this.MatchingNamespaceOfTypes.AddRange( rule.MatchingNamespaceOfTypes ),
                MatchingCurrentNamespace = this.MatchingCurrentNamespace || rule.MatchingCurrentNamespace,
                MatchingAlways = this.MatchingAlways || rule.MatchingAlways
            };
    }
}