// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Metalama.Extensions.Architecture.Fabrics
{
    [CompileTime]
    public sealed class UsageRule
    {
        internal UsageRule() { }

        internal ImmutableArray<string> AllowedNamespaces { get; init; } = ImmutableArray<string>.Empty;

        internal ImmutableArray<Type> AllowedNamespaceOfTypes { get; init; } = ImmutableArray<Type>.Empty;

        internal ImmutableArray<Type> AllowedTypes { get; init; } = ImmutableArray<Type>.Empty;

        internal bool AllowCurrentNamespace { get; init; }

        public static UsageRule OwnNamespace { get; } = new() { AllowCurrentNamespace = true };

        public static UsageRule Empty { get; } = new();

        public static UsageRule Namespace( string ns ) => new() { AllowedNamespaces = ImmutableArray.Create( ns ) };

        public static UsageRule Namespaces( IEnumerable<string> namespaces ) => new() { AllowedNamespaces = namespaces.ToImmutableArray() };

        public static UsageRule Type( Type type ) => new() { AllowedTypes = ImmutableArray.Create( type ) };

        public static UsageRule Types( IEnumerable<Type> types ) => new() { AllowedTypes = types.ToImmutableArray() };

        public static UsageRule NamespaceOfType( Type type ) => new() { AllowedNamespaceOfTypes = ImmutableArray.Create( type ) };

        public static UsageRule NamespaceOfTypes( IEnumerable<Type> types ) => new() { AllowedNamespaceOfTypes = types.ToImmutableArray() };

        internal UsageRule( BaseUsageValidationAttribute attribute )
        {
            this.AllowedNamespaces = ImmutableArray.Create( attribute.Namespaces );
            this.AllowCurrentNamespace = this.AllowCurrentNamespace;
            this.AllowedTypes = ImmutableArray.Create( attribute.Types );
            this.AllowedNamespaceOfTypes = ImmutableArray.Create( attribute.NamespaceOfTypes );
        }

        public UsageRule Or( UsageRule rule )
            => new()
            {
                AllowedTypes = this.AllowedTypes.AddRange( rule.AllowedTypes ),
                AllowedNamespaces = this.AllowedNamespaces.AddRange( rule.AllowedNamespaces ),
                AllowedNamespaceOfTypes = this.AllowedNamespaceOfTypes.AddRange( rule.AllowedNamespaceOfTypes ),
                AllowCurrentNamespace = this.AllowCurrentNamespace || rule.AllowCurrentNamespace
            };
    }
}