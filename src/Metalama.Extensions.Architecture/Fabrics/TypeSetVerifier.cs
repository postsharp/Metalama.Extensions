// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

#pragma warning disable SA1402 // File may only contain a single type

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Implementation of <see cref="ITypeSetVerifier{T}"/>.
    /// </summary>
    [PublicAPI]
    internal class TypeSetVerifier<T> : Verifier<T>, ITypeSetVerifier<T>
        where T : class, IDeclaration
    {
        private readonly Func<IAspectReceiver<T>, IAspectReceiver<INamedType>> _getTypeReceiver;

        public TypeSetVerifier(
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

        public ITypeSetVerifier<INamedType> SelectTypesDerivedFrom( Type type, DerivedTypesOptions options = DerivedTypesOptions.Default )
            => new TypeSetVerifier<INamedType>( this.TypeReceiver.Where( t => t.DerivesFrom( type, options ) ), x => x, this.Namespace );
    }
}