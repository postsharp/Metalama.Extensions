// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;

namespace Metalama.Extensions.Architecture.Aspects;

/// <summary>
/// The base class for all attributes that validate usage.
/// </summary>
[CompileTime]
[PublicAPI]
public abstract class BaseUsageValidationAttribute : Attribute, IConditionallyInheritableAspect
{
    /// <summary>
    /// Gets the namespaces that match the rule by identifying the namespaces by their full name. Any namespace string can contain one of the following patterns: <c>*</c>
    /// (matches any identifier character, but not the dot), <c>.**.</c> (matches any sub-namespace in the middle of a namespace), <c>**.</c>
    /// (matches any sub-namespace at the beginning of a namespace) or <c>.**</c> (matches any sub-namespace at the end of a namespace -- this pattern
    /// is allowed but redundant). 
    /// </summary>
    public string[] Namespaces { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the namespaces that match the rule by specifying a set of types directly contained in the namespaces.
    /// </summary>
    public Type[] NamespaceOfTypes { get; init; } = Array.Empty<Type>();

    /// <summary>
    /// Gets the types that match the rule.
    /// </summary>
    public Type[] Types { get; init; } = Array.Empty<Type>();

    /// <summary>
    /// Gets a value indicating whether the rule is matched by the namespace of the type to which the aspect is defined.
    /// </summary>
    public bool CurrentNamespace { get; init; }

    public bool ValidateDerivedTypes { get; init; }

    /// <summary>
    /// Validates the current custom attribute and sets the aspect state.
    /// </summary>
    private protected bool ValidateAndProcessProperties( IAspectBuilder<IDeclaration> builder )
    {
        // Report an error if the custom attribute was instantiated but no property was set.
        var hasProperty = this.Namespaces is { Length: > 0 } || this.NamespaceOfTypes is { Length: > 0 } ||
                          this.Types is { Length: > 0 } || this.CurrentNamespace;

        if ( !hasProperty )
        {
            builder.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.AtLeastOnePropertyMustBeSet.WithArguments( nameof(InternalsCanOnlyBeUsedFromAttribute) ) );

            builder.SkipAspect();

            return false;
        }

        return true;
    }

    bool IConditionallyInheritableAspect.IsInheritable => this.ValidateDerivedTypes;
}