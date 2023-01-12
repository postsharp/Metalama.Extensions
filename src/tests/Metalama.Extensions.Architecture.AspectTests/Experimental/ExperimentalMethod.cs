// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Extensions.Architecture.AspectTests.Experimental.ExperimentalMethod
{
    internal class BaseClass
    {
        [Experimental]
        public virtual void ExperimentalMethod() { }

        private void OtherMethod()
        {
            // Not allowed even if it is the same type, because the type itself is not experimental.
            this.ExperimentalMethod();
        }
    }

    internal class DerivedType : BaseClass
    {
        public override void ExperimentalMethod()
        {
            base.ExperimentalMethod();
        }
    }

    internal class OtherType : BaseClass
    {
        public static void Method()
        {
            new BaseClass().ExperimentalMethod();
        }
    }
}