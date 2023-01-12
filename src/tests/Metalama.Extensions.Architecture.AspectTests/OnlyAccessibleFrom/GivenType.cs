// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType
{
    [CanOnlyBeUsedFrom( Types = new[] { typeof(AllowedClass) } )]
    internal class ConstrainedClass { }

    internal class Forbidden : ConstrainedClass { }

    internal class AllowedClass : ConstrainedClass { }
}