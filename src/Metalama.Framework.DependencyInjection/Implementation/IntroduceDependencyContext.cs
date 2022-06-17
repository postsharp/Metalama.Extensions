// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Project;

namespace Metalama.Framework.DependencyInjection.Implementation;

[CompileTime]
public abstract class DependencyContext
{
    protected DependencyContext(
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

/// <summary>
/// Represents the context in which the an aspect dependency advice is weaved. 
/// </summary>
[CompileTime]
public sealed class IntroduceDependencyContext : DependencyContext
{
    internal IntroduceDependencyContext(
        IFieldOrProperty fieldOrProperty,
        string aspectFieldOrPropertyId,
        IntroduceDependencyAttribute introduceDependencyAttribute,
        INamedType targetType,
        in ScopedDiagnosticSink diagnostics,
        IProject project ) : base( project, fieldOrProperty, introduceDependencyAttribute, targetType, diagnostics )
    {
        this.AspectFieldOrPropertyId = aspectFieldOrPropertyId;
    }

    /// <summary>
    /// Gets the template name to pass to methods of <see cref="IAdviceFactory"/>.
    /// </summary>
    public string AspectFieldOrPropertyId { get; }
}

/// <summary>
/// Represents the context in which the an aspect dependency advice is weaved. 
/// </summary>
[CompileTime]
public sealed class ImplementDependencyContext : DependencyContext
{
    internal ImplementDependencyContext(
        IProject project,
        IFieldOrProperty fieldOrProperty,
        IDependencyAttribute dependencyAttribute,
        in ScopedDiagnosticSink diagnostics ) : base( project, fieldOrProperty, dependencyAttribute, fieldOrProperty.DeclaringType, diagnostics ) { }
}