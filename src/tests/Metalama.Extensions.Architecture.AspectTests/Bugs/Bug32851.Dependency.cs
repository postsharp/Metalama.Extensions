// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Extensions.Architecture.AspectTests.Bugs.Bug32851
{
    [CanOnlyBeUsedFrom( Namespaces = new[] { "ClassLibrary2" }, ValidateDerivedTypes = true )]
    public class InternalClass
    {
    }

}
