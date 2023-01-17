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
    /// Exposes, as extension methods, the fluent API for architecture validation from fabrics. Standard extension methods are exposed by the <see cref="ArchitectureAmenderExtensions"/> class.
    /// To extend the fluent API, expose extension methods of this object.
    /// </summary>
    [CompileTime]
    [PublicAPI]
    public abstract class ArchitectureAmender
    {
        internal ArchitectureAmender() { }

        public abstract IAspectReceiver<INamedType> WithTypes();

        protected abstract IValidatorReceiver WithTargetCore();

        public IValidatorReceiver WithTarget() => this.WithTargetCore();

        public abstract string? Namespace { get; }
    }

    [PublicAPI]
    public class ArchitectureAmender<T> : ArchitectureAmender
        where T : class, IDeclaration
    {
        private readonly IAspectReceiver<T> _aspectReceiver;
        private readonly Func<T, IEnumerable<INamedType>> _getTypes;

        public ArchitectureAmender( IAspectReceiver<T> aspectReceiver, Func<T, IEnumerable<INamedType>> getTypes, string? ns = null )
        {
            this._aspectReceiver = aspectReceiver;
            this._getTypes = getTypes;
            this.Namespace = ns;
        }

        public override IAspectReceiver<INamedType> WithTypes() => this._aspectReceiver.SelectMany( this._getTypes );

        protected override IValidatorReceiver WithTargetCore() => this._aspectReceiver;

        public new IAspectReceiver<T> WithTarget() => this._aspectReceiver;

        public override string? Namespace { get; }
    }
}