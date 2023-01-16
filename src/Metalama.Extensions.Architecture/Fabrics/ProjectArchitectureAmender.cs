// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;
using System;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    public sealed class ProjectArchitectureAmender : ArchitectureAmender
    {
        public IProjectAmender Amender { get; }

        public ProjectArchitectureAmender( IProjectAmender amender )
        {
            this.Amender = amender;
        }

        public override IAspectReceiver<INamedType> WithTypes( Func<INamedType, bool>? filter = null )
            => this.Amender.With( compilation => compilation.Types.Where( type => filter == null || filter( type ) ) );

        public override IValidatorReceiver ValidatorReceiver => this.Amender.With( p => p );

        public override string? Namespace => null;
    }
}