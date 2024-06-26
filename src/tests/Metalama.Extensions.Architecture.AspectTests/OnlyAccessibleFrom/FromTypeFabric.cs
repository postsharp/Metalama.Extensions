// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromTypeFabric.Allowed;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromTypeFabric
{
    internal class ConstrainedClass
    {
        private class Fabric : TypeFabric
        {
            public override void AmendType( ITypeAmender amender )
            {
                amender.CanOnlyBeUsedFrom( r => r.Type( typeof(AllowedClass) ) );
            }
        }
    }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}