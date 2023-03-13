// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Generics.GivenType_Generic
{
    [CanOnlyBeUsedFrom( Types = new[] { typeof(GenericClass<int>) } )]
    internal class ConstrainedClass { }

    internal class GenericClass<T>
    {
        // Forbidden
        public static void CreateConstrainedClassInstance()
        {
            _ = new ConstrainedClass();
        }
    }
}