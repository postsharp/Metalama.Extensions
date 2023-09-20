// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using System;

namespace Metalama.Extensions.DependencyInjection;

[CompileTime]
public sealed class DependencyInjectionFrameworkRegistration : IHierarchicalOptionItem
{
    public DependencyInjectionFrameworkRegistration( Type type, int? priority = null )
    {
        this.Type = type;
        this.Priority = priority;
    }

    object IHierarchicalOptionItem.GetKey() => this.Type;

    object IOverridable.OverrideWith( object options, in HierarchicalOptionsOverrideContext context )
        => new DependencyInjectionFrameworkRegistration( this.Type, ((DependencyInjectionFrameworkRegistration) options).Priority ?? this.Priority );

    public Type Type { get; }

    public int? Priority { get; }
}