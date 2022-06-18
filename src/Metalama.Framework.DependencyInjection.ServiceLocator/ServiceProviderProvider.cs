// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using System;

namespace Metalama.Framework.DependencyInjection.ServiceLocator
{
    public static class ServiceProviderProvider
    {
        public static Func<IServiceProvider> ServiceProvider { get; set; } = () => EmptyServiceProvider.Instance;
    }
}