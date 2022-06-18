// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using Metalama.Framework.Fabrics;

namespace Metalama.Framework.DependencyInjection.ServiceLocator;

public class Fabric : TransitiveProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Project.DependencyInjectionOptions().RegisterFramework( new ServiceLocatorDependencyInjectionFramework() );
    }
}