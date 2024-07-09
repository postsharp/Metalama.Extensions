// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

#pragma warning disable CS0649

namespace Metalama.Extensions.Architecture.AspectTests.Predicates.Or
{
    public class Fabric : NamespaceFabric
    {
        public override void AmendNamespace( INamespaceAmender amender )
        {
            amender
                .CanOnlyBeUsedFrom( r => r.Type( "**.*A*" ).Or().Type( "**.*B*" ) );
        }
    }

    internal class C { }

    internal class A
    {
        // Allowed.
        public C F;
    }

    internal class B
    {
        // Allowed.
        public C F;
    }

    internal class AB
    {
        // Allowed.
        public C F;
    }

    internal class D
    {
        // Not Allowed.
        public C F;
    }
}