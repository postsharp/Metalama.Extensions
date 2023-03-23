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
    /// Extension methods that verify the architecture. These methods extend the <see cref="TypeArchitectureVerifier{T}"/> class, which is returned
    /// by the <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/> method of the <see cref="AmenderExtensions"/> class.
    /// </summary>
    [CompileTime]
    [PublicAPI]
    public static class ArchitectureVerifierExtensions
    {
        public static void Experimental( this IArchitectureVerifier<IDeclaration> verifier )
        {
            verifier.Receiver.AddAspect( _ => new ExperimentalAttribute() );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from a different context than the ones matching the specified predicate. 
        /// </summary>
        public static void CanOnlyBeUsedFrom(
            this IArchitectureVerifier<IDeclaration> verifier,
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
            this IArchitectureVerifier<IDeclaration> verifier,
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
            this ITypeArchitectureVerifier<IDeclaration> verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            var predicateBuilder = new ReferencePredicateBuilder( verifier );
            var builtPredicate = predicate( predicateBuilder );
            var internalPredicate = builtPredicate;
            var nonInternalPredicate = predicateBuilder.HasFamilyAccess().Or( builtPredicate );

            verifier.TypeReceiver
                .Where( t => t.Accessibility == Accessibility.Internal )
                .ValidateReferences( new ReferencePredicateValidator( internalPredicate, description, referenceKinds ) );

            verifier.TypeReceiver
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
            this ITypeArchitectureVerifier<IDeclaration> verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            var predicateBuilder = new ReferencePredicateBuilder( verifier );
            var builtPredicate = predicate( predicateBuilder );
            var internalPredicate = builtPredicate;
            var nonInternalPredicate = predicateBuilder.HasFamilyAccess().Or( builtPredicate.Not() );

            verifier.TypeReceiver
                .Where( t => t.Accessibility == Accessibility.Internal )
                .ValidateReferences( new ReferencePredicateValidator( internalPredicate, description, referenceKinds ) );

            verifier.TypeReceiver
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
        public static void DerivedTypesMustRespectNamingConvention( this ITypeArchitectureVerifier<IDeclaration> verifier, string pattern )
        {
            verifier.Receiver.ValidateReferences( NamingConventionValidator.CreateStarPatternValidator( pattern ) );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a regular expression.
        /// </summary>
        public static void DerivedTypesMustRespectRegexNamingConvention( this ITypeArchitectureVerifier<IDeclaration> verifier, string pattern )
        {
            verifier.Receiver.ValidateReferences( NamingConventionValidator.CreateRegexValidator( pattern ) );
        }

        /// <summary>
        /// Represents a fluent <see cref="TypeArchitectureVerifier{T}"/> that allows to validate code using a given assembly referenced by the current compilation.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="TypeArchitectureVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="assemblyName">The name of the assembly, without version and public key.</param>
        public static ITypeArchitectureVerifier<IAssembly> WithReferencedAssembly( this ITypeArchitectureVerifier<ICompilation> verifier, string assemblyName )
            => new TypeArchitectureVerifier<IAssembly>(
                verifier.Receiver.SelectMany( c => c.ReferencedAssemblies.OfName( assemblyName ) ),
                x => x.SelectMany( a => a.Types ) );

        public static ITypeArchitectureVerifier<INamedType> Select<T>( this IArchitectureVerifier<T> verifier, Func<T, INamedType> func )
            where T : class, IDeclaration
            => new TypeArchitectureVerifier<INamedType>( verifier.Receiver.Select( func ), x => x, verifier.Namespace );

        public static ITypeArchitectureVerifier<INamedType> SelectMany<T>( this IArchitectureVerifier<T> verifier, Func<T, IEnumerable<INamedType>> func )
            where T : class, IDeclaration
            => new TypeArchitectureVerifier<INamedType>( verifier.Receiver.SelectMany( func ), x => x, verifier.Namespace );

        public static IArchitectureVerifier<TOut> Select<TIn, TOut>( this IArchitectureVerifier<TIn> verifier, Func<TIn, TOut> func )
            where TIn : class, IDeclaration
            where TOut : class, IDeclaration
            => new ArchitectureVerifier<TOut>( verifier.Receiver.Select( func ), verifier.Namespace );

        public static IArchitectureVerifier<TOut> SelectMany<TIn, TOut>( this IArchitectureVerifier<TIn> verifier, Func<TIn, IEnumerable<TOut>> func )
            where TIn : class, IDeclaration
            where TOut : class, IDeclaration
            => new ArchitectureVerifier<TOut>( verifier.Receiver.SelectMany( func ), verifier.Namespace );

        public static IArchitectureVerifier<T> Where<T>( this IArchitectureVerifier<T> verifier, Func<T, bool> predicate )
            where T : class, IDeclaration
            => new ArchitectureVerifier<T>( verifier.Receiver.Where( predicate ), verifier.Namespace );

        public static ITypeArchitectureVerifier<INamedType> Types( this ITypeArchitectureVerifier<IDeclaration> verifier )
            => new TypeArchitectureVerifier<INamedType>( verifier.TypeReceiver, x => x, verifier.Namespace );

        public static ITypeArchitectureVerifier<INamedType> Where<T>( this ITypeArchitectureVerifier<INamedType> verifier, Func<INamedType, bool> predicate )
            where T : class, IDeclaration
            => new TypeArchitectureVerifier<INamedType>(
                verifier.TypeReceiver.Where( predicate ),
                x => x,
                verifier.Namespace );

        /// <summary>
        /// Represents a fluent <see cref="ArchitectureVerifier{T}"/> that allows to validate code referencing given types.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="ArchitectureVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="types">A list of types.</param>
        public static ITypeArchitectureVerifier<INamedType> WithTypes( this IArchitectureVerifier<ICompilation> verifier, IEnumerable<Type> types )
            => new TypeArchitectureVerifier<INamedType>(
                verifier.Receiver.SelectMany( _ => types.Select( t => (INamedType) TypeFactory.GetType( t ) ) ),
                x => x );

        /// <summary>
        /// Represents a fluent <see cref="ArchitectureVerifier{T}"/> that allows to validate code referencing given types.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="ArchitectureVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="types">A list of types.</param>
        public static ITypeArchitectureVerifier<INamedType> WithTypes( this IArchitectureVerifier<ICompilation> verifier, params Type[] types )
            => verifier.WithTypes( (IEnumerable<Type>) types );
    }
}