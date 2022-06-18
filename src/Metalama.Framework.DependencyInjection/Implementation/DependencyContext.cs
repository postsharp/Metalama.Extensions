// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Project;

namespace Metalama.Framework.DependencyInjection.Implementation;

/// <summary>
/// The base class for <see cref="IntroduceDependencyContext"/> and <see cref="ImplementDependencyContext"/>, which represent the context
/// in which a dependency is being implemented or introduced. 
/// </summary>
[CompileTime]
public abstract class DependencyContext
{
    private protected DependencyContext(
        IProject project,
        IFieldOrProperty fieldOrProperty,
        IDependencyAttribute dependencyAttribute,
        INamedType targetType,
        ScopedDiagnosticSink diagnostics )
    {
        this.Project = project;
        this.FieldOrProperty = fieldOrProperty;
        this.DependencyAttribute = dependencyAttribute;
        this.TargetType = targetType;
        this.Diagnostics = diagnostics;
    }

    /// <summary>
    /// Gets the current project.
    /// </summary>
    public IProject Project { get; }

    /// <summary>
    /// Gets the advice field or property in the aspect type.
    /// </summary>
    public IFieldOrProperty FieldOrProperty { get; }

    /// <summary>
    /// Gets the <see cref="IntroduceDependencyAttribute"/> applied to <see cref="FieldOrProperty"/>.
    /// </summary>
    public IDependencyAttribute DependencyAttribute { get; }

    /// <summary>
    /// Gets the type into which the dependency should be weaved.
    /// </summary>
    public INamedType TargetType { get; }

    /// <summary>
    /// Gets object that allows to report diagnostics.
    /// </summary>
    public ScopedDiagnosticSink Diagnostics { get; }
}