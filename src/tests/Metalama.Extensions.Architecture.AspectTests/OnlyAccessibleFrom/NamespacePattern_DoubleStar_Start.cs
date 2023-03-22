// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamespacePattern_DoubleStar_Start
{
    [CanOnlyBeUsedFrom( Namespaces = new[] { "**.Allowed" } )]
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

        internal class ForbiddenClass2 : ConstrainedClass { }
    }
}