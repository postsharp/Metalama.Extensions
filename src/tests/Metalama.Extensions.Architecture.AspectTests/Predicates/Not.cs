﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;

#pragma warning disable CS0649

namespace Metalama.Extensions.Architecture.AspectTests.Predicates.Not
{
    public class Fabric : NamespaceFabric
    {
        public override void AmendNamespace( INamespaceAmender amender )
        {
            amender
                .CanOnlyBeUsedFrom( r => r.Not().Type( "**.*A*" ).And().Not().Type( "**.*B*" ) );
        }
    }

    internal class C { }

    internal class A
    {
        // Not Allowed.
        public C F;
    }

    internal class B
    {
        // Not Allowed.
        public C F;
    }

    internal class AB
    {
        // Not Allowed.
        public C F;
    }

    internal class D
    {
        // Allowed.
        public C F;
    }
}