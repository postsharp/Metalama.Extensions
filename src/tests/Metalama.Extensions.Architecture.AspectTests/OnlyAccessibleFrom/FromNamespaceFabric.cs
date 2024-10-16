﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromNamespaceFabric.ChildNs.Allowed;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.FromNamespaceFabric
{
    public class Fabric : NamespaceFabric
    {
        public override void AmendNamespace( INamespaceAmender amender )
        {
            amender.CanOnlyBeUsedFrom( r => r.NamespaceOf( typeof(AllowedClass) ) );
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