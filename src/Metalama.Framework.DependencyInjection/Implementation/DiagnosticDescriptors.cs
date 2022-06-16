// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Framework.DependencyInjection.Implementation;

[CompileTime]
internal static class DiagnosticDescriptors
{
    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        NoDependencyInjectionFrameworkRegistered = new(
            "LAMA0701",
            Severity.Error,
            "Metalama.DependencyInjection",
            "No dependency injection framework has been registered." );

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        NoSuitableDependencyInjectionFramework = new(
            "LAMA0702",
            Severity.Error,
            "Metalama.DependencyInjection",
            "None of the registered dependency injection frameworks can handle a dependency." );

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        MoreThanOneSuitableDependencyInjectionFramework = new(
            "LAMA0703",
            Severity.Error,
            "Metalama.DependencyInjection",
            "More than one dependency injection framework can handle a dependency and no Selector has been specified." );

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        NoSelectedDependencyInjectionFramework = new(
            "LAMA0704",
            Severity.Error,
            "Metalama.DependencyInjection",
            "The DependencyInjectionOptions.Selector implementation did not select any framework." );
}