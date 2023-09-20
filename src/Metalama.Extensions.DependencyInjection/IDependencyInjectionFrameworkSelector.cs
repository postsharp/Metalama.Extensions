// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.DependencyInjection.Implementation;
using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;
using System.Collections.Immutable;

namespace Metalama.Extensions.DependencyInjection;

[CompileTime]
public interface IDependencyInjectionFrameworkSelector : ICompileTimeSerializable
{
    IDependencyInjectionFramework SelectFramework( DependencyProperties properties, ImmutableArray<IDependencyInjectionFramework> eligibleFrameworks );
}