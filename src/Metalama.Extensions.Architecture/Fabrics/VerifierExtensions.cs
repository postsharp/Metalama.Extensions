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
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Extension methods that verify the architecture. These methods extend the <see cref="IVerifier{T}"/> interface, which is returned
    /// by the <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/> method of the <see cref="AmenderExtensions"/> class.
    /// </summary>
    [CompileTime]
    [PublicAPI]
    [Obsolete]
    public static class VerifierExtensions
    {
        /// <summary>
        /// Reports a warning when the target declaration is used by a declaration that it not itself marked as experimental.
        /// </summary>
        public static void Experimental( this IVerifier<IDeclaration> verifier )
        {
            verifier.Receiver.AddAspect( _ => new ExperimentalAttribute() );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from a different context than the ones matching the specified predicate. 
        /// </summary>
        public static void CanOnlyBeUsedFrom(
            this IVerifier<IDeclaration> verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> scope,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            verifier.Receiver.ValidateOutboundReferences(
                new ReferencePredicateValidator( scope( new ReferencePredicateBuilder( verifier ) ), description, referenceKinds ) );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from the context matching the specified predicate.
        /// </summary>
        public static void CannotBeUsedFrom(
            this IVerifier<IDeclaration> verifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> scope,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
        {
            verifier.Receiver
                .ValidateOutboundReferences(
                    new ReferencePredicateValidator( scope( new ReferencePredicateBuilder( verifier ) ).Not(), description, referenceKinds ) );
        }

        private static void VerifyInternalsAccess(
            this ITypeSetVerifier<IDeclaration> setVerifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> scope,
            string? description,
            ReferenceKinds referenceKinds,
            Func<ReferencePredicate, ReferencePredicate> transformPredicate )
        {
            var predicateBuilder = new ReferencePredicateBuilder( setVerifier );
            var builtPredicate = scope( predicateBuilder );
            var typePredicate = builtPredicate;
            var memberPredicate = predicateBuilder.HasFamilyAccess().Or( transformPredicate( builtPredicate ) );

            var typeValidator = new ReferencePredicateValidator( typePredicate, description, referenceKinds );

            setVerifier.TypeReceiver
                .Where( t => t.Accessibility == Accessibility.Internal )
                .ValidateOutboundReferences( typeValidator );

            var memberValidator = new ReferencePredicateValidator( memberPredicate, description, referenceKinds );

            setVerifier.TypeReceiver
                .Where( t => t.Accessibility != Accessibility.Internal )
                .SelectMany(
                    t => t.Members()
                        .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
                .ValidateOutboundReferences( memberValidator );

            setVerifier.TypeReceiver
                .Where( t => t.Accessibility != Accessibility.Internal )
                .SelectMany(
                    t => t.Properties.Where( p => p.Accessibility is Accessibility.Public or Accessibility.Protected )
                        .SelectMany( p => p.Accessors )
                        .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
                .ValidateOutboundReferences( memberValidator );
        }

        /// <summary>
        /// Reports a warning when any of the internal APIs of the current scope in used from a different context than the one allowed,
        /// except if this concept has access to the type using inheritance rules.
        /// </summary>
        public static void InternalsCanOnlyBeUsedFrom(
            this ITypeSetVerifier<IDeclaration> setVerifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> scope,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
            => setVerifier.VerifyInternalsAccess( scope, description, referenceKinds, x => x );

        /// <summary>
        /// Reports a warning when any of the internal APIs of the current scope in used from a different context different than the one allowed,
        /// except if this concept has access to the type using inheritance rules.
        /// </summary>
        public static void InternalsCannotBeUsedFrom(
            this ITypeSetVerifier<IDeclaration> setVerifier,
            Func<ReferencePredicateBuilder, ReferencePredicate> scope,
            string? description = null,
            ReferenceKinds referenceKinds = ReferenceKinds.All )
            => setVerifier.VerifyInternalsAccess( scope, description, referenceKinds, x => x.Not() );

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a star pattern, i.e. where the <c>*</c> matches any sequence of characters, even empty.
        /// </summary>
        public static void DerivedTypesMustRespectNamingConvention(
            this ITypeSetVerifier<IDeclaration> setVerifier,
            string pattern,
            Func<ReferencePredicateBuilder, ReferencePredicate>? exclusions = null )
        {
            setVerifier.Receiver.ValidateOutboundReferences(
                DerivedTypeNamingConventionValidator.CreateStarPatternValidator(
                    pattern,
                    ReferencePredicateBuilder.Build( exclusions, setVerifier.Receiver, ReferenceEndRole.Origin ) ) );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a regular expression.
        /// </summary>
        public static void DerivedTypesMustRespectRegexNamingConvention(
            this ITypeSetVerifier<IDeclaration> setVerifier,
            string pattern,
            Func<ReferencePredicateBuilder, ReferencePredicate>? exclusions = null )
        {
            setVerifier.Receiver.ValidateOutboundReferences(
                DerivedTypeNamingConventionValidator.CreateRegexValidator(
                    pattern,
                    ReferencePredicateBuilder.Build( exclusions, setVerifier.Receiver, ReferenceEndRole.Origin ) ) );
        }

        /// <summary>
        /// Reports a warning when the target declaration does not respect the given naming convention, with the asterisk character (<c>*</c>)
        /// matching any substring.
        /// </summary>
        public static void MustRespectNamingConvention( this IVerifier<INamedDeclaration> verifier, string pattern )
        {
            verifier.MustRespectRegexNamingConvention( NamingConventionHelper.StarPatternToRegex( pattern ) );
        }

        /// <summary>
        /// Reports a warning when the target declaration does not respect the given naming convention, given as a regular expression.
        /// </summary>
        public static void MustRespectRegexNamingConvention( this IVerifier<INamedDeclaration> verifier, string pattern )
        {
            var regex = new Regex( pattern );

            verifier.Receiver.Where( d => !regex.IsMatch( d.Name ) )
                .ReportDiagnostic( d => ArchitectureDiagnosticDefinitions.NamingConventionViolation.WithArguments( (d, d.DeclarationKind, pattern) ) );
        }

        /// <summary>
        /// Represents a fluent <see cref="TypeSetVerifier{T}"/> that allows to validate code using a given assembly referenced by the current compilation.
        /// This method can only be used in a <see cref="ProjectFabric"/>.
        /// </summary>
        /// <param name="setVerifier">The <see cref="TypeSetVerifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="assemblyName">The name of the assembly, without version and public key.</param>
        public static ITypeSetVerifier<IAssembly> WithReferencedAssembly( this ITypeSetVerifier<ICompilation> setVerifier, string assemblyName )
            => new TypeSetVerifier<IAssembly>(
                setVerifier.Receiver.SelectMany( c => c.ReferencedAssemblies.OfName( assemblyName ) ),
                x => x.SelectMany( a => a.Types ),
                setVerifier.AssemblyName,
                null );

        /// <summary>
        /// Selects a single type of the current <see cref="ICompilation"/> or <see cref="INamespace"/>.
        /// </summary>
        public static ITypeSetVerifier<INamedType> Select<T>( this IVerifier<T> verifier, Func<T, INamedType> func )
            where T : class, IDeclaration
            => new TypeSetVerifier<INamedType>( verifier.Receiver.Select( func ), x => x, verifier.AssemblyName, verifier.Namespace );

        /// <summary>
        /// Selects types of the current <see cref="ICompilation"/> or <see cref="INamespace"/>.
        /// </summary>
        public static ITypeSetVerifier<INamedType> SelectMany<T>( this IVerifier<T> verifier, Func<T, IEnumerable<INamedType>> func )
            where T : class, IDeclaration
            => new TypeSetVerifier<INamedType>( verifier.Receiver.SelectMany( func ), x => x, verifier.AssemblyName, verifier.Namespace );

        /// <summary>
        /// Selects a member of the current declaration.
        /// </summary>
        public static IVerifier<TOut> Select<TIn, TOut>( this IVerifier<TIn> verifier, Func<TIn, TOut> func )
            where TIn : class, IDeclaration
            where TOut : class, IDeclaration
            => new Verifier<TOut>( verifier.Receiver.Select( func ), verifier.AssemblyName, verifier.Namespace );

        /// <summary>
        /// Selects several members of the current declaration.
        /// </summary>
        public static IVerifier<TOut> SelectMany<TIn, TOut>( this IVerifier<TIn> verifier, Func<TIn, IEnumerable<TOut>> func )
            where TIn : class, IDeclaration
            where TOut : class, IDeclaration
            => new Verifier<TOut>( verifier.Receiver.SelectMany( func ), verifier.AssemblyName, verifier.Namespace );

        /// <summary>
        /// Filters the declarations in the current set.
        /// </summary>
        public static IVerifier<T> Where<T>( this IVerifier<T> verifier, Func<T, bool> predicate )
            where T : class, IDeclaration
            => new Verifier<T>( verifier.Receiver.Where( predicate ), verifier.AssemblyName, verifier.Namespace );

        /// <summary>
        /// Gets the set of types in the current <see cref="ICompilation"/> or <see cref="INamespace"/>.
        /// </summary>
        public static ITypeSetVerifier<INamedType> Types( this ITypeSetVerifier<IDeclaration> verifier )
            => new TypeSetVerifier<INamedType>( verifier.TypeReceiver, x => x, verifier.AssemblyName, verifier.Namespace );

        /// <summary>
        /// Gets a new set of types obtained by filtering the current set.
        /// </summary>
        public static ITypeSetVerifier<INamedType> Where( this ITypeSetVerifier<INamedType> verifier, Func<INamedType, bool> predicate )
            => new TypeSetVerifier<INamedType>(
                verifier.TypeReceiver.Where( predicate ),
                x => x,
                verifier.AssemblyName,
                verifier.Namespace );

        /// <summary>
        /// Gets a set of types in the current <see cref="ICompilation"/>, where types are given as an enumeration of <see cref="Type"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="Verifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="types">A list of types.</param>
        public static ITypeSetVerifier<INamedType> SelectTypes( this IVerifier<ICompilation> verifier, IEnumerable<Type> types )
            => new TypeSetVerifier<INamedType>(
                verifier.Receiver.SelectMany( _ => types.Select( t => (INamedType) TypeFactory.GetType( t ) ) ),
                x => x,
                verifier.AssemblyName,
                null );

        /// <summary>
        /// Gets a set of types in the current <see cref="ICompilation"/>, where types are given as a list of array of <see cref="Type"/>.
        /// </summary>
        /// <param name="verifier">The <see cref="Verifier{T}"/> returned by <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/>.</param>
        /// <param name="types">A list of types.</param>
        public static ITypeSetVerifier<INamedType> SelectTypes( this IVerifier<ICompilation> verifier, params Type[] types )
            => verifier.SelectTypes( (IEnumerable<Type>) types );
    }
}