// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using System;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    internal class TypeArchitectureAmender : ArchitectureAmender
    {
        private readonly ITypeAmender _amender;

        public TypeArchitectureAmender( ITypeAmender amender )
        {
            this._amender = amender;
        }

        public override IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter )
            => this._amender.With( type => new[] { type }.Where( type => filter == null || filter( type ) ) );
    }
}