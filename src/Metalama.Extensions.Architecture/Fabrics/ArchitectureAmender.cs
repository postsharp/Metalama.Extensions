﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;
using System;

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

        /// <summary>
        /// Gets the <see cref="IAspectReceiver{TDeclaration}"/> for the upstream <see cref="IAmender{T}"/>. This method should be used
        /// only when you want to extend the architecture validation API.
        /// </summary>
        public abstract IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter = null );

        public abstract IValidatorReceiver ValidatorReceiver { get; }

        public abstract string? Namespace { get; }
    }
}