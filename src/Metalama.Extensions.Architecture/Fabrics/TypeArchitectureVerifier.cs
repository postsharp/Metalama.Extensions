// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

#pragma warning disable SA1402 // File may only contain a single type

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Implementation of <see cref="ITypeArchitectureVerifier{T}"/>.
    /// </summary>
    [PublicAPI]
    internal sealed class TypeArchitectureVerifier<T> : ArchitectureVerifier<T>, ITypeArchitectureVerifier<T>
        where T : class, IDeclaration
    {
        private readonly Func<IAspectReceiver<T>, IAspectReceiver<INamedType>> _getTypeReceiver;

        public TypeArchitectureVerifier(
            IAspectReceiver<T> receiver,
            Func<IAspectReceiver<T>, IAspectReceiver<INamedType>> getTypeReceiver,
            string? ns = null ) : base(
            receiver,
            ns )
        {
            this._getTypeReceiver = getTypeReceiver;
        }

        /// <inheritdoc />
        public IAspectReceiver<INamedType> TypeReceiver => this._getTypeReceiver( this.Receiver );
    }
}