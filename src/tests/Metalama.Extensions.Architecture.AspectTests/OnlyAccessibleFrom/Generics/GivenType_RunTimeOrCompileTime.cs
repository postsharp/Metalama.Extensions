// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Generics.GivenType_RunTimeOrCompileTime
{
    [CanOnlyBeUsedFrom( Types = new[] { typeof(GenericClass<>) } )]
    [RunTimeOrCompileTime]
    internal class ConstrainedClass { }

    internal class GenericClass
    {
        public static void CreateConstrainedClassInstance()
        {
            _ = new ConstrainedClass();
        }
    }

    [RunTimeOrCompileTime]
    internal class GenericClass<T>
    {
        public static void CreateConstrainedClassInstance()
        {
            _ = new ConstrainedClass();
        }
    }

    internal class GenericClass<T1, T2>
    {
        public static void CreateConstrainedClassInstance()
        {
            _ = new ConstrainedClass();
        }
    }

    internal class GenericClass<T1, T2, T3>
    {
        public static void CreateConstrainedClassInstance()
        {
            _ = new ConstrainedClass();
        }
    }
}