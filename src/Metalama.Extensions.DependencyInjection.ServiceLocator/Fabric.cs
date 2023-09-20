// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Framework.Options;

namespace Metalama.Extensions.DependencyInjection.ServiceLocator;

internal class Fabric : TransitiveProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Outbound.Configure(
            _ => new DependencyInjectionOptions()
            {
                FrameworkRegistrations = new HierarchicalOptionItemCollection<DependencyInjectionFrameworkRegistration>(
                    new DependencyInjectionFrameworkRegistration( typeof(ServiceLocatorDependencyInjectionFramework) ) )
            } );
    }
}