// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_Method
{
    internal class ConstrainedClass
    {
        [CanOnlyBeUsedFrom( Types = new[] { typeof(AllowedClass) } )]
        public void ConstrainedMethod() { }
    }

    internal class ForbiddenClass
    {
        private static void CallMethod()
        {
            new ConstrainedClass().ConstrainedMethod();
        }
    }

    internal class AllowedClass
    {
        private static void CallMethod()
        {
            new ConstrainedClass().ConstrainedMethod();
        }
    }
}