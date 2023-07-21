﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.DependencyInjection.Implementation;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Extensions.DependencyInjection;

/// <summary>
/// Custom attribute that, when be applied to a field or automatic property of an aspect, means that this field or property is a service dependency
/// that introduced into the target type and handled by a dependency injection framework. Contrarily to <see cref="DependencyAttribute"/> that can be used
/// in user code, this attribute can be used only in an aspect. 
/// </summary>
/// <remarks>
///  The implementation of this custom attribute depends on the selected dependency injection framework.
/// </remarks>
public class IntroduceDependencyAttribute : DeclarativeAdviceAttribute, IDependencyAttribute
{
    private bool? _isLazy;
    private bool? _isRequired;

    public sealed override void BuildAdvice( IMemberOrNamedType templateMember, string templateMemberId, IAspectBuilder<IDeclaration> builder )
    {
        var context = new IntroduceDependencyContext(
            (IFieldOrProperty) templateMember,
            this,
            builder.Target.GetClosestNamedType()!,
            builder.Diagnostics,
            builder.Project );

        if ( !builder.Project.DependencyInjectionOptions().TryGetFramework( context, out var framework ) )
        {
            builder.SkipAspect();

            return;
        }

        framework.IntroduceDependency( context, builder.WithTarget( builder.Target.GetClosestNamedType()! ) );
    }

    /// <summary>
    /// Gets the value of the <see cref="IsLazy"/> if it has been assigned, or <c>null</c> if it has not been assigned.
    /// </summary>
    public bool? GetIsLazy() => this._isLazy;

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
    public bool? GetIsRequired() => this._isRequired;
}