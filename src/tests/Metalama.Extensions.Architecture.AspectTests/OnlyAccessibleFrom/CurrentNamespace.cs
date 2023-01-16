// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.CurrentNamespace;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.CurrentNamespace
{
    [CanOnlyBeUsedFrom( CurrentNamespace = true )]
    internal class ConstrainedClass { }

    // Valid because of same namespace.
    internal class ClassInSameNamespace : ConstrainedClass { }

    namespace ChildNamespace
    {
        // Valid because in child namespace.
        internal class AllowedClass : ConstrainedClass { }
    }
}

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.CurrentNamespace_Other
{
    // Invalid because in other namespace.
    internal class ForbiddenClass : ConstrainedClass { }
}