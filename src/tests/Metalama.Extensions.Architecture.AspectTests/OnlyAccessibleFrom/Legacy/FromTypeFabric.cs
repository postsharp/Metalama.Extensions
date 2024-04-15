// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.FromTypeFabric.Allowed;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

#pragma warning disable CS0612, CS0618 // Type or member is obsolete

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.FromTypeFabric
{
    internal class ConstrainedClass
    {
        private class Fabric : TypeFabric
        {
            public override void AmendType( ITypeAmender amender )
            {
                amender.Verify().CanOnlyBeUsedFrom( r => r.Type( typeof(AllowedClass) ) );
            }
        }
    }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}