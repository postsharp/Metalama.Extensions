// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;
using System;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    public sealed class TypeArchitectureAmender : ArchitectureAmender
    {
        public ITypeAmender Amender { get; }

        public TypeArchitectureAmender( ITypeAmender amender )
        {
            this.Amender = amender;
        }

        public override IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter = null )
            => this.Amender.With( type => new[] { type }.Where( t => filter == null || filter( t ) ) );

        public override IValidatorReceiver ValidatorReceiver => this.Amender.With( t => t );

        public override string? Namespace => this.Amender.Type.Namespace.FullName;
    }
}