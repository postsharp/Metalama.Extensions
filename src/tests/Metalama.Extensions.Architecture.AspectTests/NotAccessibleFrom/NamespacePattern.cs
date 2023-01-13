// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.NotAccessibleFrom.NamespacePattern
{
    [CannotBeUsedFrom( Namespaces = new[] { "**.Forbidden" } )]
    internal class ConstrainedClass { }

    internal class AllowedClass : ConstrainedClass { }

    namespace Forbidden
    {
        internal class ForbiddenClass : ConstrainedClass { }
    }
}