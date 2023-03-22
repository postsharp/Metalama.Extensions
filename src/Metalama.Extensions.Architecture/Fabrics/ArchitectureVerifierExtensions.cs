// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Framework.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Extension methods that verify the architecture. These methods extend the <see cref="ArchitectureVerifier"/> class, which is returned
    /// by the <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/> method of the <see cref="AmenderExtensions"/> class.
    /// </summary>
    [CompileTime]
    [PublicAPI]
    public static class ArchitectureVerifierExtensions
    {
        public static void Experimental( this ArchitectureVerifier verifier )
        {
            verifier.WithTypes().AddAspect( _ => new ExperimentalAttribute() );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from a different context than the ones matching the specified predicate. 
        /// </summary>
        public static void CanOnlyBeUsedFrom(
            this ArchitectureVerifier verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            verifier.Receiver.ValidateReferences(
                new ReferencePredicateValidator( predicate( new ReferencePredicateBuilder( verifier ) ), description, referenceKinds ) );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from the context matching the specified predicate.
        /// </summary>
        public static void CannotBeUsedFrom(
            this ArchitectureVerifier verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            verifier.Receiver
                .ValidateReferences(
                    new ReferencePredicateValidator( predicate( new ReferencePredicateBuilder( verifier ) ).Not(), description, referenceKinds ) );
        }

        /// <summary>
        /// Reports a warning when any of the internal APIs of the current scope in used from a different context than the one allowed,
        /// except if this concept has access to the type using inheritance rules.
        /// </summary>
        public static void InternalsCanOnlyBeUsedFrom(
            this ArchitectureVerifier verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            var predicateBuilder = new ReferencePredicateBuilder( verifier );
            var builtPredicate = predicate( predicateBuilder );
            var internalPredicate = builtPredicate;
            var nonInternalPredicate = predicateBuilder.HasFamilyAccess().Or( builtPredicate );

            verifier.WithTypes()
                .Where( t => t.Accessibility == Accessibility.Internal )
                .ValidateReferences( new ReferencePredicateValidator( internalPredicate, description, referenceKinds ) );

            verifier.WithTypes()
                .Where( t => t.Accessibility != Accessibility.Internal )
                .SelectMany(
                    t => t.Members()
                        .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
                .ValidateReferences( new ReferencePredicateValidator( nonInternalPredicate, description, referenceKinds ) );
        }

        /// <summary>
        /// Reports a warning when any of the internal APIs of the current scope in used from a different context different than the one allowed,
        /// except if this concept has access to the type using inheritance rules.
        /// </summary>
        public static void InternalsCannotBeUsedFrom(
            this ArchitectureVerifier verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            var predicateBuilder = new ReferencePredicateBuilder( verifier );
            var builtPredicate = predicate( predicateBuilder );
            var internalPredicate = builtPredicate;
            var nonInternalPredicate = predicateBuilder.HasFamilyAccess().Or( builtPredicate.Not() );

            verifier.WithTypes()
                .Where( t => t.Accessibility == Accessibility.Internal )
                .ValidateReferences( new ReferencePredicateValidator( internalPredicate, description, referenceKinds ) );

            verifier.WithTypes()
                .Where( t => t.Accessibility != Accessibility.Internal )
                .SelectMany(
                    t => t.Members()
                        .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
                .ValidateReferences( new ReferencePredicateValidator( nonInternalPredicate, description, referenceKinds ) );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a star pattern, i.e. where the <c>*</c> matches any sequence of characters, even empty.
        /// </summary>
        public static void DerivedTypesMustRespectNamingConvention( this ArchitectureVerifier verifier, string pattern )
        {
            verifier.Receiver.ValidateReferences( NamingConventionValidator.CreateStarPatternValidator( pattern ) );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a regular expression.
        /// </summary>
        public static void DerivedTypesMustRespectRegexNamingConvention( this ArchitectureVerifier verifier, string pattern )
        {
            verifier.Receiver.ValidateReferences( NamingConventionValidator.CreateRegexValidator( pattern ) );
        }

        /// <summary>
        /// Represents a fluent <see cref="ArchitectureVerifier{T}"/> that allows to validate code using a given assembly referenced by the current compilation.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="ArchitectureVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="assemblyName">The name of the assembly, without version and public key.</param>
        public static ArchitectureVerifier<IAssembly> WithReferencedAssembly( this ArchitectureVerifier<ICompilation> verifier, string assemblyName )
            => new( verifier.Receiver.SelectMany( c => c.ReferencedAssemblies.OfName( assemblyName ) ), a => a.Types );

        /// <summary>
        /// Represents a fluent <see cref="ArchitectureVerifier{T}"/> that allows to validate code referencing given types.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="ArchitectureVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="types">A list of types.</param>
        public static ArchitectureVerifier<INamedType> WithTypes( this ArchitectureVerifier<ICompilation> verifier, IEnumerable<Type> types )
            => new( verifier.Receiver.SelectMany( _ => types.Select( t => (INamedType) TypeFactory.GetType( t ) ) ), t => new[] { t } );

        /// <summary>
        /// Represents a fluent <see cref="ArchitectureVerifier{T}"/> that allows to validate code referencing given types.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="ArchitectureVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="types">A list of types.</param>
        public static ArchitectureVerifier<INamedType> WithTypes( this ArchitectureVerifier<ICompilation> verifier, params Type[] types )
            => verifier.WithTypes( (IEnumerable<Type>) types );
    }
}