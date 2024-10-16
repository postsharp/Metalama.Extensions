﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.FromNamespaceFabric.Allowed;
using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.FromNamespaceFabric.Constrained;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

#pragma warning disable CS0612, CS0618 // Type or member is obsolete

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.FromNamespaceFabric
{
    namespace Constrained
    {
        public class Fabric : NamespaceFabric
        {
            public override void AmendNamespace( INamespaceAmender amender )
            {
                amender.Verify().CanOnlyBeUsedFrom( r => r.Namespace( typeof(AllowedClass).Namespace! ) );
            }
        }

        internal class ConstrainedClass { }
    }

    namespace Forbidden
    {
        internal class ForbiddenClass : ConstrainedClass { }
    }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}