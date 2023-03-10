// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Extensions.Architecture;

[CompileTime]
internal static class ArchitectureDiagnosticDefinitions
{
    public static readonly DiagnosticDefinition<(IDeclaration ReferencedDeclaration, DeclarationKind ReferencedDeclarationKind, string? OptionalSpace, string?
            Description)>
        ExperimentalApi =
            new(
                "LAMA0900",
                Severity.Warning,
                "The '{0}' {1} is experimental.{2}{3}",
                "The API is experimental." );

    public static readonly DiagnosticDefinition<(IDeclaration Interface, string AssemblyName)> InternalImplement = new(
        "LAMA0901",
        Severity.Warning,
        "The '{0}' interface can only be implemented by the '{1}` project or from another project having access to its internals.",
        "The interface can be only implemented by the project that defines it or by another project having access to its internals." );

    public static readonly DiagnosticDefinition<(string Pattern, string ErrorMessage)> InvalidRegex = new(
        "LAMA0902",
        Severity.Error,
        "The string `{0}` is not a valid regular expression: {1}",
        "The string is not a valid regular expression." );

    public static readonly DiagnosticDefinition<(INamedType ReferencingType, INamedType BaseType, string Pattern)> NamingConventionViolation = new(
        "LAMA0903",
        Severity.Error,
        "The type '{0}' does not respect the naming convention set on the base class or interface '{1}'. The type name should match the \"{2}\" pattern.",
        "The type does not respect the naming convention set on the base class or interface." );

    public static readonly DiagnosticDefinition<string> AtLeastOnePropertyMustBeSet = new(
        "LAMA0904",
        Severity.Error,
        "At least one property of the '{0}' custom attribute must be set.",
        "At least one property of the attribute must be set." );

    public static readonly DiagnosticDefinition<(IDeclaration ValidatedDeclaration, DeclarationKind DeclarationKind, string UsageKind, INamedType ReferencingType, string?
            OptionalSpace, string? Description)>
        OnlyAccessibleFrom = new(
            "LAMA0905",
            Severity.Warning,
            "The '{0}' {1} cannot be {2} by the '{3}' type.{4}{5}",
            "The declaration cannot be used from this context because of an architecture constraint." );
}