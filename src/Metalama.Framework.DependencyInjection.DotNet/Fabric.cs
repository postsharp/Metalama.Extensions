// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Fabrics;

namespace Metalama.Framework.DependencyInjection.DotNet;

/// <summary>
/// A <see cref="TransitiveProjectFabric"/> that registers <see cref="DotNetDependencyInjectionFramework"/>.
/// </summary>
public class Fabric : TransitiveProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
        => amender.Project.DependencyInjectionOptions().RegisterFramework( new DotNetDependencyInjectionFramework() );
}