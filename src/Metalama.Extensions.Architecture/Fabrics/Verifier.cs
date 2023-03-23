// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Extensions.Architecture.Fabrics;

/// <summary>
/// Implementation of <see cref="IVerifier{T}"/>.
/// </summary>
internal class Verifier<T> : IVerifier<T>
    where T : class, IDeclaration
{
    public IAspectReceiver<T> Receiver { get; }

    public string? Namespace { get; }

    public Verifier( IAspectReceiver<T> receiver, string? ns )
    {
        this.Receiver = receiver;
        this.Namespace = ns;
    }
}