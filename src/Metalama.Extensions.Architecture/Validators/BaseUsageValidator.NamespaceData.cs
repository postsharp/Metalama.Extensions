// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;
using System.Collections.Immutable;

namespace Metalama.Extensions.Architecture.Aspects;

internal partial class BaseUsageValidator
{
    [CompileTime]
    public sealed class NamespaceData : ICompileTimeSerializable
    {
        public bool AnyType { get; set; } = true;

        public ImmutableHashSet<string> SpecificTypes { get; set; } = ImmutableHashSet<string>.Empty;
    }
}