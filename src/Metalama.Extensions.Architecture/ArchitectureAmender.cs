// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

namespace Metalama.Extensions.Architecture
{
    /// <summary>
    /// Exposes, as extension methods, the fluent API for architecture validation from fabrics. Standard extension methods are exposed by the <see cref="ArchitectureAmenderExtensions"/> class.
    /// To extend the fluent API, expose extension methods of this object.
    /// </summary>
    [CompileTime]
    public abstract class ArchitectureAmender
    {
        /// <summary>
        /// Gets the <see cref="IAspectReceiver{TDeclaration}"/> for the upstream <see cref="IAmemder"/>. This method should be used
        /// only when you want to extend the architecture validation API.
        /// </summary>
        public abstract IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter = null );
    }
}