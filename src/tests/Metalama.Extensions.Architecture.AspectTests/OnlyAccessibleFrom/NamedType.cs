// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamedType
{
    [CanOnlyBeUsedFrom( TypeNames = new[] { "Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.NamedType.Allowed.AllowedClass" } )]
    internal class ConstrainedClass { }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}