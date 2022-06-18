// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using System;

namespace Metalama.Framework.DependencyInjection.ServiceLocator;

internal class EmptyServiceProvider : IServiceProvider
{
    public static EmptyServiceProvider Instance { get; } = new();

    public object? GetService( Type serviceType ) => null;
}