// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.InternalsAccess.PropertyAccessors
{
    [InternalsCannotBeUsedFrom( Types = new[] { typeof(ForbiddenClass) } )]
    public class C
    {
        public string P { get; internal set; }
    }

    internal class AllowedClass
    {
        public void M()
        {
            var c = new C();
            c.P = c.P;
        }
    }

    internal class ForbiddenClass
    {
        public void M()
        {
            var c = new C();

            // This should report a single error.
            c.P = c.P;
        }
    }
}