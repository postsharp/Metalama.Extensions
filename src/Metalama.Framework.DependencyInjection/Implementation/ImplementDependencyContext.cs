// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Project;

namespace Metalama.Framework.DependencyInjection.Implementation;

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