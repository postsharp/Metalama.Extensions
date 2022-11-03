// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Extensions.Multicast;

[CompileTime]
internal interface IMulticastImplementation<T>
    where T : class, IDeclaration
{
    void AddAspects( IAspectBuilder<T> builder );
}