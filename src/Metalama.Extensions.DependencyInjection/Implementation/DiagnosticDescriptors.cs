// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Extensions.DependencyInjection.Implementation;

[CompileTime]
internal static class DiagnosticDescriptors
{
    private const string _category = "Metalama.Extensions.DependencyInjection";

    // Diagnostics need to be declared in an aspect or fabric at the moment.

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        NoDependencyInjectionFrameworkRegistered = new(
            "LAMA0701",
            Severity.Error,
            "No dependency injection framework can handle the dependency '{0}' in type '{1}'.",
            "No dependency injection framework has been registered.",
            _category );

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        NoSuitableDependencyInjectionFramework = new(
            "LAMA0702",
            Severity.Error,
            "None of the registered dependency injection frameworks can handle the dependency '{0}' in type '{1}'.",
            "None of the registered dependency injection frameworks can handle a dependency.",
            _category );

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        MoreThanOneSuitableDependencyInjectionFramework = new(
            "LAMA0703",
            Severity.Error,
            "More than one dependency injection framework can handle the dependency '{0}' in type '{1}' and no Selector has been specified.",
            "More than one dependency injection framework can handle a dependency and no Selector has been specified.",
            _category );

    internal static readonly DiagnosticDefinition<(IFieldOrProperty Dependency, INamedType TargetType)>
        NoSelectedDependencyInjectionFramework = new(
            "LAMA0704",
            Severity.Error,
            "The DependencyInjectionOptions.Selector implementation did not select any framework for dependency '{0}' in type '{1}'.",
            "The DependencyInjectionOptions.Selector implementation did not select any framework.",
            _category );

    public override void AmendProject( IProjectAmender amender ) { }
}