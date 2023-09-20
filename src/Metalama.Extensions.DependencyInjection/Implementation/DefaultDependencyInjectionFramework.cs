// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using System;

namespace Metalama.Extensions.DependencyInjection.Implementation;

/// <summary>
/// The default implementation of <see cref="IDependencyInjectionFramework"/>. It pulls dependencies from all constructors and use <see cref="Func{TResult}"/>
/// to accept lazy dependencies.
/// </summary>
public class DefaultDependencyInjectionFramework : IDependencyInjectionFramework
{
    /// <inheritdoc />
    public virtual bool CanHandleDependency( DependencyProperties properties, in ScopedDiagnosticSink diagnostics ) => !properties.IsStatic;

    /// <inheritdoc />
    public bool TryIntroduceDependency(
        DependencyProperties properties,
        IAspectBuilder<INamedType> aspectBuilder,
        out IFieldOrProperty? dependencyFieldOrProperty )
    {
        return this.GetStrategy( properties ).TryIntroduceDependency( aspectBuilder, out dependencyFieldOrProperty );
    }

    /// <inheritdoc />
    public bool TryImplementDependency( DependencyProperties properties, IAspectBuilder<IFieldOrProperty> aspectBuilder )
    {
        return this.GetStrategy( properties ).TryImplementDependency( aspectBuilder );
    }

    /// <summary>
    /// Gets an instance of the <see cref="DefaultDependencyInjectionStrategy"/> class for a given context.
    /// </summary>
    protected virtual DefaultDependencyInjectionStrategy GetStrategy( DependencyProperties properties )
        => properties.IsLazy
            ? new LazyDependencyInjectionStrategy( properties )
            : new DefaultDependencyInjectionStrategy( properties );
}