// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamespacePattern_DoubleStar
{
    [CanOnlyBeUsedFrom( Namespaces = new[] { "Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamespacePattern_DoubleStar.**.Allowed" } )]
    internal class ConstrainedClass { }

    internal class ForbiddenClass : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }

        namespace Child
        {
            internal class AllowedClass : ConstrainedClass { }
        }
    }

    namespace Intermediate
    {
        namespace Allowed
        {
            internal class AllowedClass : ConstrainedClass { }

            namespace Child
            {
                internal class AllowedClass : ConstrainedClass { }
            }
        }

        internal class ForbiddenClass : ConstrainedClass { }
    }
}