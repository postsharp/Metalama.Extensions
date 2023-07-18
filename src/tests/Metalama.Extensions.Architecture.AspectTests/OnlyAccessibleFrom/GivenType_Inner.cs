// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_Inner
{
    [CanOnlyBeUsedFrom( Types = new[] { typeof(AllowedClass.InnerClass) } )]
    internal abstract class ConstrainedClass { }

    internal class ForbiddenClass
    {
        internal class InnerClass
        {
            private ConstrainedClass _field;
        }
    }

    internal abstract class AllowedClass
    {
        internal class InnerClass
        {
            private ConstrainedClass _field;
        }
    }
}