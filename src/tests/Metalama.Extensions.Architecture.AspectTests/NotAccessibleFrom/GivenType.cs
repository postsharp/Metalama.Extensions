// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Extensions.Architecture.AspectTests.NotAccessibleFrom.GivenType
{
    [CannotBeUsedFrom( Types = new[] { typeof(ForbiddenClass) } )]
    internal class ConstrainedClass { }

    internal class AllowedClass : ConstrainedClass { }

    internal class ForbiddenClass : ConstrainedClass { }
}