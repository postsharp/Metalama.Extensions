// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Project;

namespace Metalama.Framework.DependencyInjection.Implementation;

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