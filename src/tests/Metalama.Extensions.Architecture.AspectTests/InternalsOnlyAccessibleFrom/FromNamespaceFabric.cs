// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.InternalsOnlyAccessibleFrom.FromNamespaceFabric.ConstrainedNs;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.AspectTests.InternalsOnlyAccessibleFrom.FromNamespaceFabric
{
    namespace ConstrainedNs
    {
        public class Fabric : NamespaceFabric
        {
            public override void AmendNamespace( INamespaceAmender amender )
            {
                amender.Verify().InternalsCanOnlyBeUsedFrom( r => r.CurrentNamespace() );
            }
        }

        internal class InternalClass { }

        public class PublicClass
        {
            public static void PublicMethod() { }

            internal static void InternalMethod() { }
        }
    }

    namespace UnfriendNs
    {
        internal class ForbiddenClassWithAllowedCalls
        {
            public static void SomeMethod()
            {
                // Allowed because public.
                PublicClass.PublicMethod();
            }
        }

        internal class ForbiddenClassWithForbiddenCalls
        {
            public static void SomeMethod()
            {
                // Forbidden because internal.
                _ = typeof(InternalClass);
                PublicClass.InternalMethod();
            }
        }
    }
}