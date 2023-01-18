// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;

namespace Metalama.Extensions.Architecture.AspectTests.InternalImplement.InternalImplementTest
{
    [InternalOnlyImplement]
    public interface IInterface
    {
        void Method();
    }
}