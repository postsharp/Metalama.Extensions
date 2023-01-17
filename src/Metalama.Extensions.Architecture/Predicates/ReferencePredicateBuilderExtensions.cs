// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Metalama.Extensions.Architecture.Predicates;

[CompileTime]
public static class ReferencePredicateBuilderExtensions
{
    public static ReferencePredicate Or( this ReferencePredicate predicate, Func<ReferencePredicateBuilder, ReferencePredicate> otherPredicate )
    {
        if ( predicate.Builder == null )
        {
            throw new InvalidOperationException( "No ReferencePredicateBuilder available." );
        }

        return new OrPredicate( predicate, otherPredicate( predicate.Builder ) );
    }

    public static ReferencePredicate Or( this ReferencePredicate predicate, ReferencePredicate otherPredicate ) => new OrPredicate( predicate, otherPredicate );

    public static ReferencePredicate And( this ReferencePredicate predicate, Func<ReferencePredicateBuilder, ReferencePredicate> otherPredicate )
    {
        if ( predicate.Builder == null )
        {
            throw new InvalidOperationException( "No ReferencePredicateBuilder available." );
        }

        return new AndPredicate( predicate, otherPredicate( predicate.Builder ) );
    }

    public static ReferencePredicate And( this ReferencePredicate predicate, ReferencePredicate otherPredicate )
        => new AndPredicate( predicate, otherPredicate );

    public static ReferencePredicate Not( this ReferencePredicate predicate ) => new NotPredicate( predicate );

    public static ReferencePredicate Always( this ReferencePredicateBuilder builder ) => new AlwaysPredicate( builder );

    public static ReferencePredicate Any( this ReferencePredicateBuilder builder, Func<ReferencePredicateBuilder, IEnumerable<ReferencePredicate>> predicates )
        => new AnyPredicate( predicates( builder ).ToImmutableArray(), builder );

    public static ReferencePredicate Namespace( this ReferencePredicateBuilder builder, string ns ) => new ReferencingNamespacePredicate( ns, builder );

    public static ReferencePredicate CurrentNamespace( this ReferencePredicateBuilder builder )
    {
        if ( builder.Namespace == null )
        {
            throw new InvalidOperationException( "There is no namespace in the current context." );
        }

        return new ReferencingNamespacePredicate( builder.Namespace, builder );
    }

    public static ReferencePredicate NamespaceOf( this ReferencePredicateBuilder builder, Type type )
        => new ReferencingNamespacePredicate(
            type.Namespace ?? throw new ArgumentOutOfRangeException( nameof(type), $"The type {type.FullName} has no namespace." ),
            builder );

    public static ReferencePredicate Type( this ReferencePredicateBuilder builder, Type type ) => new ReferencingTypePredicate( type, builder );

    public static ReferencePredicate HasFamilyAccess( this ReferencePredicateBuilder builder ) => new HasFamilyAccessPredicate( builder );
}