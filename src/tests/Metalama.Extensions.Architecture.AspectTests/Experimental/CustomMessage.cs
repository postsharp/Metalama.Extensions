﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.Experimental.CustomMessage
{
    [Experimental( "This is the custom message." )]
    public class C { }

    internal class D : C { }
}