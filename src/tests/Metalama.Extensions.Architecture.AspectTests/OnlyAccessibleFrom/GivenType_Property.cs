// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

#pragma warning disable IDE0059

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.GivenType_Property
{
    internal class ConstrainedClass
    {
        [CanOnlyBeUsedFrom( Types = new[] { typeof(AllowedClass) } )]
        public object? ConstrainedProperty { get; set; }
    }

    internal class ForbiddenClass
    {
        private static void GetAndSet()
        {
            var o = new ConstrainedClass();
            var v = o.ConstrainedProperty;
            o.ConstrainedProperty = new object();
        }
    }

    internal class AllowedClass
    {
        private static void GetAndSet()
        {
            var o = new ConstrainedClass();
            var v = o.ConstrainedProperty;
            o.ConstrainedProperty = new object();
        }
    }
}