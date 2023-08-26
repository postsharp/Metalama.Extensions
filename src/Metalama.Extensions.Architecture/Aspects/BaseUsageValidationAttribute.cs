// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Validation;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ReferencePredicate = Metalama.Extensions.Architecture.Predicates.ReferencePredicate;

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
    /// Gets the full names of the types that match the rule. Any type string can contain one of the following patterns: <c>*</c>
    /// (matches any identifier character, but not the dot), <c>.**.</c> (matches any sub-namespace in the middle of a full type name), <c>**.</c>
    /// (matches any sub-namespace at the beginning of the full type name) or <c>.**</c> (matches any sub-namespace and any type name at the end of a namespace). 
    /// </summary>
    public string[] TypeNames { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets a value indicating whether the rule is matched by the namespace of the type to which the attribute is defined.
    /// </summary>
    public bool CurrentNamespace { get; init; }

    /// <summary>
    /// Gets a value indicating whether the rule is matched by the assembly in which the attribute is defined.
    /// </summary>
    public bool CurrentAssembly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the types that are derived from the target type should also be validated, e.g. whether the aspect is inheritable.
    /// </summary>
    public bool ValidateDerivedTypes { get; init; }

    /// <summary>
    /// Gets an optional description message appended to the warning message.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the kinds of references that must be validated. The default value is <see cref="Metalama.Framework.Validation.ReferenceKinds.All"/>.
    /// </summary>
    public ReferenceKinds ReferenceKinds { get; init; } = ReferenceKinds.All;

    /// <summary>
    /// Gets a <see cref="Type"/>, derived from <see cref="ReferencePredicate"/>, that determines exclusions for the current
    /// architecture rule. Specifically, no warning will be reported if the  <see cref="ReferencePredicate.IsMatch"/> method
    /// of the <see cref="ReferencePredicate"/> returns <c>true</c>. This type must have a default constructor.
    /// </summary>
    public Type? ExclusionPredicateType { get; init; }

    /// <summary>
    /// Creates a <see cref="ReferencePredicate"/> based on the properties of the custom attributes.
    /// </summary>
    protected bool TryCreatePredicate( IAspectBuilder<IMemberOrNamedType> builder, [NotNullWhen( true )] out ReferencePredicate? predicate )
    {
        var currentNamespace = builder.Target.GetClosestNamedType()!.Namespace;

        var predicates = ImmutableArray.CreateBuilder<ReferencePredicate>();

        if ( !this.AddPredicatesFromAttributes( currentNamespace, predicates.Add, builder.Diagnostics ) )
        {
            builder.SkipAspect();
            predicate = null;

            return false;
        }

        switch ( predicates.Count )
        {
            case 0:
                builder.Diagnostics.Report( ArchitectureDiagnosticDefinitions.AtLeastOnePropertyMustBeSet.WithArguments( this.GetType().Name ) );

                builder.SkipAspect();

                predicate = null;

                return false;

            case 1:
                predicate = predicates[0];

                return true;

            default:
                predicate = new AnyPredicate( predicates.ToImmutable() );

                break;
        }

        return true;
    }

    protected bool TryCreateExclusionPredicate( IAspectBuilder<IMemberOrNamedType> builder, out ReferencePredicate? predicate )
    {
        if ( this.ExclusionPredicateType != null )
        {
            if ( !ExclusionPredicateTypeHelper.ValidateExclusionPredicateType( this.ExclusionPredicateType, builder.Diagnostics ) )
            {
                builder.SkipAspect();
                predicate = null;

                return false;
            }

            predicate = ExclusionPredicateTypeHelper.GetExclusionPredicate( this.ExclusionPredicateType );
        }
        else
        {
            predicate = null;
        }

        return true;
    }

    bool IConditionallyInheritableAspect.IsInheritable => this.ValidateDerivedTypes;

    /// <summary>
    /// Adds the predicates defined by the properties of the current custom attribute. 
    /// </summary>
    /// <param name="currentNamespace">The namespace in which the current attribute is used.</param>
    /// <param name="addPredicate">A delegate to call to add a predicate. If many predicates are added, they will be combined
    ///     with the <see cref="ReferencePredicateExtensions.Or(Metalama.Extensions.Architecture.Predicates.ReferencePredicate,System.Func{Metalama.Extensions.Architecture.Predicates.ReferencePredicateBuilder,Metalama.Extensions.Architecture.Predicates.ReferencePredicate})"/>
    ///     method.</param>
    /// <param name="diagnostics"></param>
    protected virtual bool AddPredicatesFromAttributes(
        INamespace currentNamespace,
        Action<ReferencePredicate> addPredicate,
        in ScopedDiagnosticSink diagnostics )
    {
        if ( this.CurrentNamespace )
        {
            addPredicate( new ReferencingNamespacePredicate( currentNamespace.FullName ) );
        }

        if ( this.CurrentAssembly )
        {
            addPredicate( new ReferencingAssemblyPredicate( currentNamespace.DeclaringAssembly.Identity.Name ) );
        }

        foreach ( var ns in this.Namespaces )
        {
            addPredicate( new ReferencingNamespacePredicate( ns ) );
        }

        foreach ( var type in this.NamespaceOfTypes )
        {
            if ( type.Namespace == null )
            {
                continue;
            }

            addPredicate( new ReferencingNamespacePredicate( type.Namespace ) );
        }

        switch ( this.Types.Length )
        {
            case 0:
                break;

            case 1:
                addPredicate( new ReferencingTypePredicate( this.Types[0] ) );

                break;

            default:
                addPredicate( new AnyReferencingTypePredicate( this.Types ) );

                break;
        }

        foreach ( var typeName in this.TypeNames )
        {
            addPredicate( new ReferencingTypeNamePredicate( typeName ) );
        }

        return true;
    }
}