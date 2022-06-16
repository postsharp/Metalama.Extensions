// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Framework.DependencyInjection.Implementation;

/// <summary>
/// Represents the context in which the an aspect dependency advice is weaved. 
/// </summary>
[CompileTime]
public sealed class DependencyInjectionContext
{
    internal DependencyInjectionContext(
        IFieldOrProperty aspectFieldOrProperty,
        string aspectFieldOrPropertyId,
        DependencyAttribute dependencyAttribute,
        INamedType targetType,
        in ScopedDiagnosticSink diagnostics )
    {
        this.AspectFieldOrProperty = aspectFieldOrProperty;
        this.AspectFieldOrPropertyId = aspectFieldOrPropertyId;
        this.DependencyAttribute = dependencyAttribute;
        this.TargetType = targetType;
        this.Diagnostics = diagnostics;
    }

    /// <summary>
    /// Gets the advice field or property in the aspect type.
    /// </summary>
    public IFieldOrProperty AspectFieldOrProperty { get; }

    /// <summary>
    /// Gets the template name to pass to methods of <see cref="IAdviceFactory"/>.
    /// </summary>
    public string AspectFieldOrPropertyId { get; }

    /// <summary>
    /// Gets the <see cref="DependencyAttribute"/> applied to <see cref="AspectFieldOrProperty"/>.
    /// </summary>
    public DependencyAttribute DependencyAttribute { get; }

    /// <summary>
    /// Gets the type into which the dependency should be weaved.
    /// </summary>
    public INamedType TargetType { get; }

    /// <summary>
    /// Gets object that allows to report diagnostics.
    /// </summary>
    public ScopedDiagnosticSink Diagnostics { get; }
}