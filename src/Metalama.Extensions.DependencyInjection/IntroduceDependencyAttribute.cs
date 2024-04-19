﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
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
[PublicAPI]
public class IntroduceDependencyAttribute : DeclarativeAdviceAttribute
{
    private bool? _isLazy;
    private bool? _isRequired;

    protected virtual DependencyProperties ToProperties( IFieldOrProperty templateFieldOrProperty, INamedType targetType )
    {
        return new DependencyProperties(
            targetType,
            templateFieldOrProperty.Type,
            templateFieldOrProperty.Name,
            templateFieldOrProperty.IsStatic,
            this._isRequired,
            this._isLazy,
            templateFieldOrProperty.DeclarationKind );
    }

    public sealed override void BuildAdvice( IMemberOrNamedType templateMember, string templateMemberId, IAspectBuilder<IDeclaration> builder )
    {
        // Suppress warnings on the aspect field.
        builder.Diagnostics.Suppress( DiagnosticDescriptors.NonNullableFieldMustContainValue, templateMember );
        builder.Diagnostics.Suppress( DiagnosticDescriptors.PrivateMemberIsUnused, templateMember );

        if ( !builder.TryIntroduceDependency(
                this.ToProperties( (IFieldOrProperty) templateMember, builder.Target.GetClosestNamedType()! ),
                out _ ) )
        {
            builder.SkipAspect();
        }
    }

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
}