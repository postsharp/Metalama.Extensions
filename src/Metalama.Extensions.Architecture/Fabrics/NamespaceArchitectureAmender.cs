// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using System;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    internal class NamespaceArchitectureAmender : ArchitectureAmender
    {
        private readonly INamespaceAmender _amender;
        private readonly bool _includeChildNamespaces;

        public NamespaceArchitectureAmender( INamespaceAmender amender, bool includeChildNamespaces )
        {
            this._amender = amender;
            this._includeChildNamespaces = includeChildNamespaces;
        }

        public override IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter )
            => this._amender.With(
                ns =>
                {
                    var types = this._includeChildNamespaces ? ns.DescendantsAndSelf().SelectMany( n => n.Types ) : ns.Types;

                    return filter == null ? types : types.Where( filter );
                } );
    }
}