// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamespacePattern_SimpleStar
{
    [CanOnlyBeUsedFrom( Namespaces = new[] { "Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamespacePattern_SimpleStar.Allowed*" } )]
    internal class ConstrainedClass { }

    internal class ForbiddenClass1 : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }

        namespace Child
        {
            internal class AllowedClass : ConstrainedClass { }
        }
    }

    namespace ForbiddenNs
    {
        internal class ForbiddenClass2 : ConstrainedClass { }

        namespace Child
        {
            internal class ForbiddenClass3 : ConstrainedClass { }
        }
    }
}