// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Aspects;

namespace Metalama.Extensions.Architecture.Predicates;

[CompileTime]
public sealed class ReferencePredicateBuilder
{
    public ReferencePredicateBuilder( ArchitectureVerifier verifier )
    {
        this.Namespace = verifier.Namespace;
    }

    public string? Namespace { get; }
}