// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using System;

namespace Metalama.Framework.DependencyInjection.ServiceLocator
{
    /// <summary>
    /// Exposes the global service provider.
    /// </summary>
    public static class ServiceProviderProvider
    {
        /// <summary>
        /// Gets or sets a delegate that provides a <see cref="IServiceProvider"/>. This delegate is called from types that consume dependencies.
        /// The default implementation is to return a <see cref="IServiceProvider"/> that contains no service.
        /// </summary>
        public static Func<IServiceProvider> ServiceProvider { get; set; } = () => EmptyServiceProvider.Instance;
    }
}