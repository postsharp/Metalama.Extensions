// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Serialization;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Predicates;

/// <summary>
/// The base class for predicates that applies to a <see cref="ReferenceValidationContext"/>. Standard extension methods
/// are provided on <see cref="ReferencePredicateExtensions"/>.
/// You can create your own predicates by deriving a new class from this one and exposing the API
/// as a extension methods of <see cref="ReferencePredicateBuilder"/>. 
/// </summary>
/// <remarks>
/// <para>
/// This class implements <see cref="ICompileTimeSerializable"/>. All predicates must therefore be serializable.
/// </para>
/// </remarks>
[PublicAPI]
public abstract class ReferencePredicate : ICompileTimeSerializable
{
    /// <summary>
    /// Gets the <see cref="ReferencePredicateBuilder"/> from which the current predicate was built,
    /// or <c>null</c> if the object was null without a <see cref="ReferencePredicateBuilder"/>.
    /// </summary>
    [NonCompileTimeSerialized]
    protected internal ReferencePredicateBuilder? Builder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicate"/> class.
    /// </summary>
    /// <remarks>
    /// <para>If you are implementing this class and want your implementation to be usable from the predicate fluent API, call the second constructor
    /// overload and pass the <see cref="ReferencePredicateBuilder"/>.</para>
    /// </remarks>
    protected ReferencePredicate() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferencePredicate"/> class and provides a <see cref="ReferencePredicateBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ReferencePredicateBuilder"/> from which the predicate was built, or <c>null</c>.
    /// Setting this parameter allows the new object to use used in fluent expressions like <see cref="ReferencePredicateExtensions.Or(Metalama.Extensions.Architecture.Predicates.ReferencePredicate,System.Func{Metalama.Extensions.Architecture.Predicates.ReferencePredicateBuilder,Metalama.Extensions.Architecture.Predicates.ReferencePredicate})"/>.</param>
    protected ReferencePredicate( ReferencePredicateBuilder? builder )
    {
        this.Builder = builder;
    }

    /// <summary>
    /// Gets a value indicating whether the predicate matches the given <see cref="ReferenceValidationContext"/>.
    /// </summary>
    public abstract bool IsMatch( ReferenceValidationContext context );

    public virtual bool IsMatch( in ReferenceInstance referenceInstance ) => true;

    public abstract ReferenceGranularity Granularity { get; }
}