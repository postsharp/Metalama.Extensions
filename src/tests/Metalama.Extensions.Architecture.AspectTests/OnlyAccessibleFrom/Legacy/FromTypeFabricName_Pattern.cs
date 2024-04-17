﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.FromTypeNameFabric_Pattern
{
    internal class ConstrainedClass
    {
        private class Fabric : TypeFabric
        {
            public override void AmendType( ITypeAmender amender )
            {
#pragma warning disable CS0612 , CS0618 // Type or member is obsolete
                amender.Verify().CanOnlyBeUsedFrom( r => r.Type( "**.Allowed.*" ) );
            }
        }
    }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}