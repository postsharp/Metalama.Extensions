// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.NamingConvention.StarNamingConvention
{
    [DerivedTypesMustRespectNamingConvention( "*Factory" )]
    internal class BaseClass { }

    internal class CorrectNameFactory : BaseClass { }

    internal class IncorrectName : BaseClass { }
}