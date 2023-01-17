// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;
using System.Collections.Generic;

#pragma warning disable SA1402 // File may only contain a single type

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Exposes, as extension methods, the fluent API for architecture validation from fabrics. Standard extension methods are exposed by the <see cref="ArchitectureVerifierExtensions"/> class.
    /// To extend the fluent API, expose extension methods of this object. This class is the weakly-typed base of <see cref="ArchitectureVerifier{T}"/>.
    /// </summary>
    [CompileTime]
    [PublicAPI]
    public abstract class ArchitectureVerifier
    {
        internal ArchitectureVerifier() { }

        /// <summary>
        /// Gets an <see cref="IAspectReceiver{TDeclaration}"/> that selects all types in the scope.
        /// </summary>
        /// <returns></returns>
        public abstract IAspectReceiver<INamedType> WithTypes();

        /// <summary>
        /// Gets a weakly-typed <see cref="IValidatorReceiver"/> that allows to add validators to the target declarations in the current scope.
        /// </summary>
        public IValidatorReceiver WithTarget() => this.WithTargetCore();

        private protected abstract IValidatorReceiver WithTargetCore();

        /// <summary>
        /// Gets the full name of the namespace in the current scope, or <c>null</c> if no namespace is relevant to the current scope.
        /// </summary>
        public abstract string? Namespace { get; }
    }

    /// <summary>
    /// Exposes, as extension methods, the fluent API for architecture validation from fabrics. Standard extension methods are exposed by the <see cref="ArchitectureVerifierExtensions"/> class.
    /// To extend the fluent API, expose extension methods of this object. This class is the strongly-typed implementation of <see cref="ArchitectureVerifier"/>.
    /// </summary>
    [PublicAPI]
    public sealed class ArchitectureVerifier<T> : ArchitectureVerifier
        where T : class, IDeclaration
    {
        private readonly IAspectReceiver<T> _aspectReceiver;
        private readonly Func<T, IEnumerable<INamedType>> _getTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchitectureVerifier{T}"/> class.
        /// </summary>
        /// <param name="aspectReceiver">An <see cref="IAspectReceiver{TDeclaration}"/> for the direct targets in the scope of the new <see cref="ArchitectureVerifier{T}"/>.</param>
        /// <param name="getTypes">A delegate that selects all types in the scope of the new <see cref="ArchitectureVerifier{T}"/>.</param>
        /// <param name="ns">The full name of the namespace in the current scope, or <c>null</c> if no namespace is relevant to the current scope.</param>
        public ArchitectureVerifier( IAspectReceiver<T> aspectReceiver, Func<T, IEnumerable<INamedType>> getTypes, string? ns = null )
        {
            this._aspectReceiver = aspectReceiver;
            this._getTypes = getTypes;
            this.Namespace = ns;
        }

        /// <inheritdoc />
        public override IAspectReceiver<INamedType> WithTypes() => this._aspectReceiver.SelectMany( this._getTypes );

        private protected override IValidatorReceiver WithTargetCore() => this._aspectReceiver;

        /// <summary>
        /// Gets an <see cref="IAspectReceiver{TDeclaration}"/> that allows to add validators and aspects to the target declarations in the current scope.
        /// </summary>
        public new IAspectReceiver<T> WithTarget() => this._aspectReceiver;

        /// <inheritdoc />
        public override string? Namespace { get; }
    }
}