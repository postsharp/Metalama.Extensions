// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.DependencyInjection.Implementation;
using System.Diagnostics;

namespace Metalama.Framework.DependencyInjection;

public class DependencyAttribute : FieldOrPropertyAspect, IDependencyAttribute
{
    private bool? _isLazy;
    private bool? _isRequired;

    /// <summary>
    /// Gets the value of the <see cref="IsLazy"/> if it has been assigned, or <c>null</c> if it has not been assigned.
    /// </summary>
    bool? IDependencyAttribute.GetIsLazy() => this._isLazy;

    /// <summary>
    /// Gets or sets a value indicating whether the dependency should be pulled from the container lazily, i.e. upon first use.
    /// </summary>
    public bool IsLazy
    {
        get => this._isLazy.GetValueOrDefault();
        set => this._isLazy = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dependency is required.
    /// </summary>
    public bool IsRequired
    {
        get => this._isRequired.GetValueOrDefault();
        set => this._isRequired = value;
    }

    /// <summary>
    /// Gets the value of the <see cref="IsRequired"/> if it has been assigned, or <c>null</c> if it has not been assigned.
    /// </summary>
    bool? IDependencyAttribute.GetIsRequired() => this._isRequired;

    public override void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
    {
        Debugger.Break();

        var context = new ImplementDependencyContext(
            builder.Project,
            builder.Target,
            this,
            builder.Diagnostics );

        if ( !builder.Project.DependencyInjectionOptions().TryGetFramework( context, out var framework ) )
        {
            builder.SkipAspect();

            return;
        }

        framework.ImplementDependency( context, builder );
    }
}