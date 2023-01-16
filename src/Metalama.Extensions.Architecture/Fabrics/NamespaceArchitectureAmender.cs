// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;
using System;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    public sealed class NamespaceArchitectureAmender : ArchitectureAmender
    {
        public INamespaceAmender Amender { get; }

        private readonly bool _includeChildNamespaces;

        public NamespaceArchitectureAmender( INamespaceAmender amender, bool includeChildNamespaces )
        {
            this.Amender = amender;
            this._includeChildNamespaces = includeChildNamespaces;
        }

        public override IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter = null )
            => this.Amender.With(
                ns =>
                {
                    var types = this._includeChildNamespaces ? ns.DescendantsAndSelf().SelectMany( n => n.Types ) : ns.Types;

                    return filter == null ? types : types.Where( filter );
                } );

        public override IValidatorReceiver ValidatorReceiver => this.Amender.With( ns => ns );

        public override string? Namespace => this.Amender.Namespace;
    }
}