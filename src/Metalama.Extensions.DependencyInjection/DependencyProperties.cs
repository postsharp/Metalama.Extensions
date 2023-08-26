// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;
using System;

namespace Metalama.Extensions.DependencyInjection;

/// <summary>
/// Specifications of an introduced dependency. 
/// </summary>
[CompileTime]
public record DependencyProperties(
    INamedType TargetType,
    IType DependencyType,
    string Name,
    bool IsStatic = false,
    DeclarationKind Kind = DeclarationKind.Field )
{
    public DependencyProperties(
        INamedType targetType,
        Type dependencyType,
        string name,
        bool isStatic = false,
        DeclarationKind kind = DeclarationKind.Field ) : this( targetType, TypeFactory.GetType( dependencyType ), name, isStatic, kind ) { }

    /// <summary>
    /// Gets a value indicating whether the dependency is required. When this property is set to <c>false</c>, the code will accept missing dependencies.
    /// The default value, when this property is neither specified nor overwritten, is <c>true</c>. 
    /// </summary>
    public bool? IsRequired { get; init; }

    /// <summary>
    /// Gets a value indicating whether the dependency should be resolved lazily upon first use.
    /// The default value, when this property is neither specified nor overwritten, is <c>false</c>.  
    /// </summary>
    public bool? IsLazy { get; init; }

    public IProject Project => this.TargetType.Compilation.Project;
}