// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// The base class for all attributes that validate usage.
/// </summary>
[CompileTime]
[PublicAPI]
public abstract class BaseUsageValidationAttribute : Attribute, IAspect
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

    /// <summary>
    /// Validates the current custom attribute and sets the aspect state.
    /// </summary>
    private protected bool ValidateAndProcessProperties( IAspectBuilder<IDeclaration> builder, INamespace ns )
    {
        // Report an error if the custom attribute was instantiated but no property was set.
        var hasProperty = (this.Namespaces is { Length: > 0 }) || (this.NamespaceOfTypes is { Length: > 0 }) ||
                          (this.Types is { Length: > 0 }) || this.CurrentNamespace;

        if ( !hasProperty )
        {
            builder.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.AtLeastOnePropertyMustBeSet.WithArguments( nameof(InternalsCanOnlyBeUsedFromAttribute) ) );

            builder.SkipAspect();

            return false;
        }

        // Process the attributes, optimize the data and store it in a state object that will be passed to the validation method. 
        builder.AspectState = new SerializableAccessibilityRule( this, ns, builder.Diagnostics );

        return true;
    }

    /// <summary>
    /// Determines whether the current context matches the rule of the current aspect. When is method returns <c>false</c>,
    /// the <see cref="ValidateReference"/> method reports an error.
    /// </summary>
    private protected virtual bool IsMatch( in ReferenceValidationContext context )
    {
        // Validate the type and/or namespace.
        var data = (SerializableAccessibilityRule) context.AspectState!;

        for ( var ns = context.ReferencingType.Namespace; ns != null; ns = ns.ParentNamespace )
        {
            // Check exactly named namespaces.
            if ( data.Namespaces.TryGetValue( ns.FullName, out var namespaceData ) )
            {
                if ( namespaceData.AnyType )
                {
                    return true;
                }
                else if ( namespaceData.SpecificTypes?.Contains( context.ReferencingType.Name ) == true )
                {
                    return true;
                }
            }

            // Check namespace patterns.
            if ( !data.NamespacePatterns.IsDefaultOrEmpty )
            {
                foreach ( var pattern in data.NamespacePatterns )
                {
                    if ( Regex.IsMatch( ns.FullName, pattern ) )
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Validates a reference and reports a warning if necessary.
    /// </summary>
    protected virtual void ValidateReference( in ReferenceValidationContext context )
    {
        // Do not validate inside the same type.
        if ( context.ReferencingDeclaration.IsContainedIn( context.ReferencedDeclaration.GetClosestNamedType()! ) )
        {
            return;
        }

        if ( !this.IsMatch( context ) )
        {
            // Report the error message.
            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.OnlyAccessibleFrom.WithArguments(
                    (context.ReferencedDeclaration, context.ReferencedDeclaration.DeclarationKind, this.GetType().Name) ) );
        }
    }
}