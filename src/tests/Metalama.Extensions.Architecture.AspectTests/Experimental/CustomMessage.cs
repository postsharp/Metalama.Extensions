﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Extensions.Architecture.AspectTests.Experimental.CustomMessage
{
    [Experimental( "This is the custom message." )]
    internal class C { }

    internal class D : C { }
}