// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Options;
using System;

namespace Metalama.Extensions.DependencyInjection;

/// <summary>
/// Specifications of an introduced dependency. 
/// </summary>
[CompileTime]
public record DependencyProperties
{
    public DependencyProperties(
        INamedType targetType,
        IType dependencyType,
        string name,
        bool isStatic = false,
        bool? isRequired = null,
        bool? isLazy = null,
        DeclarationKind kind = DeclarationKind.Field )
    {
        this.TargetType = targetType;
        this.DependencyType = dependencyType;
        this.Name = name;
        this.IsStatic = isStatic;
        this.Kind = kind;

        this.Options = this.TargetType.Enhancements()
            .GetOptions<DependencyInjectionOptions>()
            .OverrideWithSafe( new DependencyInjectionOptions { IsLazy = isLazy, IsRequired = isRequired }, default )!;
    }

    public DependencyProperties(
        INamedType targetType,
        Type dependencyType,
        string name,
        bool isStatic = false,
        bool? isRequired = null,
        bool? isLazy = null,
        DeclarationKind kind = DeclarationKind.Field ) : this( targetType, TypeFactory.GetType( dependencyType ), name, isStatic, isRequired, isLazy, kind ) { }

    /// <summary>
    /// Gets a value indicating whether the dependency is required. When this property is set to <c>false</c>, the code will accept missing dependencies.
    /// The default value, when this property is neither specified nor overwritten, is <c>true</c>. 
    /// </summary>
    public bool IsRequired => this.Options.IsRequired!.Value;

    /// <summary>
    /// Gets a value indicating whether the dependency should be resolved lazily upon first use.
    /// The default value, when this property is neither specified nor overwritten, is <c>false</c>.  
    /// </summary>
    public bool IsLazy => this.Options.IsLazy!.Value;

    public DependencyInjectionOptions Options { get; }

    public INamedType TargetType { get; }

    public IType DependencyType { get; }

    public string Name { get; }

    public bool IsStatic { get; }

    public DeclarationKind Kind { get; }
}