// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalama.Extensions.Architecture.AspectTests.NamingConvention.InGenericArgument;

[DerivedTypesMustRespectNamingConvention( "*Factory" )]
internal class BaseClass { }

internal class DerivedClass : BaseClass { }

internal class NotDerivedClass : IEnumerable<BaseClass>
{
    public IEnumerator<BaseClass> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}