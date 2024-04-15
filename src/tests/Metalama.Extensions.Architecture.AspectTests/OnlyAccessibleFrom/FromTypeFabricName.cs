﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromTypeNameFabric.Allowed;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromTypeNameFabric
{
    internal class ConstrainedClass
    {
        private class Fabric : TypeFabric
        {
            public override void AmendType( ITypeAmender amender )
            {
                amender.CanOnlyBeUsedFrom( r => r.Type( typeof(AllowedClass).FullName! ) );
            }
        }
    }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}