// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamedNamespace
{
    [CanOnlyBeUsedFrom( Namespaces = new[] { "Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamedNamespace.Allowed" } )]
    internal class ConstrainedClass { }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}