// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.DependencyInjection.ServiceLocator;

internal class Fabric : TransitiveProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Project.DependencyInjectionOptions().RegisterFramework( new ServiceLocatorDependencyInjectionFramework() );
    }
}