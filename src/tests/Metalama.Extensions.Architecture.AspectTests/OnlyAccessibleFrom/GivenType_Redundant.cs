// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_Redundant.Allowed;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_Redundant
{
    [CanOnlyBeUsedFrom( NamespaceOfTypes = new[] { typeof(AllowedClass) }, Types = new[] { typeof(AllowedClass) } )]
    internal class ConstrainedClass { }

    internal class Forbidden : ConstrainedClass { }

    namespace Allowed
    {
        internal class AllowedClass : ConstrainedClass { }
    }
}