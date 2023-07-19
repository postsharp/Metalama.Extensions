// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromNamespaceFabric.ChildNs.Allowed;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;
using System.Diagnostics;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromNamespaceFabric
{
    public class Fabric : NamespaceFabric
    {
        public override void AmendNamespace( INamespaceAmender amender )
        {
            amender.Verify().CanOnlyBeUsedFrom( r => r.Namespace( typeof(AllowedClass).Namespace! ) );
        }
    }

    namespace ChildNs
    {
        internal class ConstrainedClass { }

        internal class Forbidden : ConstrainedClass { }

        namespace Allowed
        {
            internal class AllowedClass : ConstrainedClass { }
        }
    }
}