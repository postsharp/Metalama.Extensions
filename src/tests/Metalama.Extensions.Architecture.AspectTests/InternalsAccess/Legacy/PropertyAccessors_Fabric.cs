// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Fabrics;
using Metalama.Extensions.Architecture.Predicates;

#pragma warning disable CS0612, CS0618 // Type or member is obsolete

namespace Metalama.Extensions.Architecture.AspectTests.InternalsAccess.Legacy.PropertyAccessors_Fabric
{
    public class C
    {
        public string P { get; internal set; }

        private class Fabric : TypeFabric
        {
            public override void AmendType( ITypeAmender amender )
            {
                amender.Verify().InternalsCannotBeUsedFrom( x => x.Type( typeof(ForbiddenClass) ) );
            }
        }
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