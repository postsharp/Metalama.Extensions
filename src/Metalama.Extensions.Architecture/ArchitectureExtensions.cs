// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Validation;
using System;
using System.Text.RegularExpressions;

namespace Metalama.Extensions.Architecture;

/// <summary>
/// Extension methods that verify the architecture. These methods extend the <see cref="IAspectReceiver{TDeclaration}"/> interface, which is returned
/// by the <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/> method of the <see cref="AmenderExtensions"/> class.
/// </summary>
[CompileTime]
[PublicAPI]
public static class ArchitectureExtensions
{
    /// <summary>
    /// Reports a warning when the target declaration is used by a declaration that it not itself marked as experimental.
    /// </summary>
    public static void Experimental( this IAspectReceiver<IDeclaration> receiver ) => receiver.AddAspect( _ => new ExperimentalAttribute() );

    /// <summary>
    /// Reports a warning when any type in the selection is used from a different context than the ones matching the specified predicate. 
    /// </summary>
    public static void CanOnlyBeUsedFrom(
        this IAspectReceiver<IDeclaration> receiver,
        Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        => receiver.Tag( _ => 0 ).CanOnlyBeUsedFromCore( ( builder, _, _ ) => predicate( builder ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any type in the selection is used from a different context than the ones matching the specified predicate.
    /// This overload supplies the selected declaration to the predicate.
    /// </summary>
    public static void CanOnlyBeUsedFrom<TDeclaration>(
        this IAspectReceiver<TDeclaration> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.Tag( d => d ).CanOnlyBeUsedFromCore( ( builder, d, _ ) => predicate( builder, d ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any type in the selection is used from a different context than the ones matching the specified predicate.
    /// This overload supplies the selected declaration and the tag (added using <see cref="IAspectReceiver{TDeclaration}.Tag"/>) to the predicate.
    /// </summary>
    public static void CanOnlyBeUsedFrom<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.CanOnlyBeUsedFromCore( predicate, description, referenceKinds );

    private static void CanOnlyBeUsedFromCore<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
    {
        var taggedReceiver = receiver
            .Tag(
                ( d, tag ) => new ReferencePredicateValidator(
                    predicate( new ReferencePredicateBuilder( ReferenceEndRole.Origin, receiver ), d, tag ),
                    description,
                    referenceKinds ) );

        taggedReceiver
                .ValidateInboundReferences( ( _, validator ) => validator );
    }

    /// <summary>
    /// Reports a warning when any type in the selection is used from the context matching the specified predicate.
    /// </summary>
    public static void CannotBeUsedFrom(
        this IAspectReceiver<IDeclaration> receiver,
        Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        => receiver
            .Tag( _ => 0 )
            .CannotBeUsedFromCore( ( builder, _, _ ) => predicate( builder ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any type in the selection is used from the context matching the specified predicate.
    /// This overload supplies the selected declaration to the predicate.
    /// </summary>
    public static void CannotBeUsedFrom<TDeclaration>(
        this IAspectReceiver<TDeclaration> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver
            .Tag( x => x )
            .CannotBeUsedFromCore( ( builder, d, _ ) => predicate( builder, d ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any type in the selection is used from the context matching the specified predicate.
    /// This overload supplies the selected declaration and the tag (added using <see cref="IAspectReceiver{TDeclaration}.Tag"/>) to the predicate.
    /// </summary>
    public static void CannotBeUsedFrom<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.CannotBeUsedFromCore( predicate, description, referenceKinds );

    private static void CannotBeUsedFromCore<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver
            .Tag( ( d, tag ) => predicate( new ReferencePredicateBuilder( ReferenceEndRole.Origin, receiver ), d, tag ).Not() )
                .ValidateInboundReferences( ( _, validator ) => new ReferencePredicateValidator( validator, description, referenceKinds ) );

    private static void VerifyInternalsAccess<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description,
        ReferenceKinds referenceKinds,
        Func<ReferencePredicate, ReferencePredicate> transformPredicate )
        where TDeclaration : class, IDeclaration
    {
        var taggedReceiver = receiver.Tag(
            ( d, tag ) =>
            {
                var predicateBuilder = new ReferencePredicateBuilder( ReferenceEndRole.Origin, receiver );
                var builtPredicate = predicate( predicateBuilder, d, tag );
                var typePredicate = builtPredicate;
                var memberPredicate = predicateBuilder.HasFamilyAccess().Or( transformPredicate( builtPredicate ) );

                var typeValidator = new ReferencePredicateValidator( typePredicate, description, referenceKinds );
                var memberValidator = new ReferencePredicateValidator( memberPredicate, description, referenceKinds );

                return new { typeValidator, memberValidator };
            } );

        var types = taggedReceiver.SelectTypes();

        var publicTypes = types
            .Where( t => t.Accessibility != Accessibility.Internal );

        // Check internal types.
        types
            .Where( t => t.Accessibility == Accessibility.Internal )
                .ValidateInboundReferences( ( _, tag ) => tag.typeValidator );

        // Check internal members of public types.
        publicTypes
            .SelectMany( t => t.Members() )
            .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal )
                .ValidateInboundReferences( ( _, tag ) => tag.memberValidator );

        // Check internal accessors of public properties.
        publicTypes
            .SelectMany( t => t.Properties )
            .Where( p => p.Accessibility is Accessibility.Public or Accessibility.Protected )
            .SelectMany( p => p.Accessors )
            .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal )
                .ValidateInboundReferences( ( _, tag ) => tag.memberValidator );
    }

    /// <summary>
    /// Reports a warning when any of the internal APIs of the current selection in used from a different context than the one allowed,
    /// except if this concept has access to the type using inheritance rules.
    /// </summary>
    public static void InternalsCanOnlyBeUsedFrom(
        this IAspectReceiver<IDeclaration> receiver,
        Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        => receiver.Tag( _ => 0 ).InternalsCanOnlyBeUsedFromCore( ( builder, _, _ ) => predicate( builder ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any of the internal APIs of the current selection in used from a different context than the one allowed,
    /// except if this concept has access to the type using inheritance rules.
    /// This overload supplies the selected declaration to the predicate.
    /// </summary>
    public static void InternalsCanOnlyBeUsedFrom<TDeclaration>(
        this IAspectReceiver<TDeclaration> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.Tag( x => x ).InternalsCanOnlyBeUsedFromCore( ( builder, d, _ ) => predicate( builder, d ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any of the internal APIs of the current selection in used from a different context than the one allowed,
    /// except if this concept has access to the type using inheritance rules.
    /// This overload supplies the selected declaration and the tag (added using <see cref="IAspectReceiver{TDeclaration}.Tag"/>) to the predicate.
    /// </summary>
    public static void InternalsCanOnlyBeUsedFrom<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => InternalsCannotBeUsedFromCore( receiver, predicate, description );

    private static void InternalsCanOnlyBeUsedFromCore<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.VerifyInternalsAccess( predicate, description, referenceKinds, x => x );

    /// <summary>
    /// Reports a warning when any of the internal APIs of the current selection in used from a different context different than the one allowed,
    /// except if this concept has access to the type using inheritance rules.
    /// </summary>
    public static void InternalsCannotBeUsedFrom(
        this IAspectReceiver<IDeclaration> receiver,
        Func<ReferencePredicateBuilder, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        => receiver.Tag( _ => 0 ).InternalsCannotBeUsedFromCore( ( builder, _, _ ) => predicate( builder ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any of the internal APIs of the current selection in used from a different context different than the one allowed,
    /// except if this concept has access to the type using inheritance rules.
    /// This overload supplies the selected declaration to the predicate. 
    /// </summary>
    public static void InternalsCannotBeUsedFrom<TDeclaration>(
        this IAspectReceiver<TDeclaration> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.Tag( x => x ).InternalsCannotBeUsedFromCore( ( builder, d, _ ) => predicate( builder, d ), description, referenceKinds );

    /// <summary>
    /// Reports a warning when any of the internal APIs of the current selection in used from a different context different than the one allowed,
    /// except if this concept has access to the type using inheritance rules.
    /// This overload supplies the selected declaration and the tag (added using <see cref="IAspectReceiver{TDeclaration}.Tag"/>) to the predicate. 
    /// </summary>
    public static void InternalsCannotBeUsedFrom<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.InternalsCannotBeUsedFromCore( predicate, description, referenceKinds );

    private static void InternalsCannotBeUsedFromCore<TDeclaration, TTag>(
        this IAspectReceiver<TDeclaration, TTag> receiver,
        Func<ReferencePredicateBuilder, TDeclaration, TTag, ReferencePredicate> predicate,
        string? description = null,
        ReferenceKinds referenceKinds = ReferenceKinds.All )
        where TDeclaration : class, IDeclaration
        => receiver.VerifyInternalsAccess( predicate, description, referenceKinds, ( x ) => x.Not() );

    /// <summary>
    /// Reports a warning when any type that inherits any type in the current selection does not follow a given convention, where the convention
    /// is given as a star pattern, i.e. where the <c>*</c> matches any sequence of characters, even empty.
    /// </summary>
    public static void DerivedTypesMustRespectNamingConvention(
        this IAspectReceiver<IDeclaration> receiver,
        string pattern,
        Func<ReferencePredicateBuilder, ReferencePredicate>? exclusions = null )
            => receiver.ValidateInboundReferences(
            DerivedTypeNamingConventionValidator.CreateStarPatternValidator(
                pattern,
                ReferencePredicateBuilder.Build( exclusions, receiver, ReferenceEndRole.Origin ) ) );

    /// <summary>
    /// Reports a warning when any type that inherits any type in the current selection does not follow a given convention, where the convention
    /// is given as a regular expression.
    /// </summary>
    public static void DerivedTypesMustRespectRegexNamingConvention(
        this IAspectReceiver<IDeclaration> receiver,
        string pattern,
        Func<ReferencePredicateBuilder, ReferencePredicate>? exclusions = null )
            => receiver.ValidateInboundReferences(
            DerivedTypeNamingConventionValidator.CreateRegexValidator(
                pattern,
                ReferencePredicateBuilder.Build( exclusions, receiver, ReferenceEndRole.Origin ) ) );

    /// <summary>
    /// Reports a warning when any declaration in the selection fails to respect the given naming convention, with the asterisk character (<c>*</c>)
    /// matching any substring.
    /// </summary>
    public static void MustRespectNamingConvention( this IAspectReceiver<INamedDeclaration> receiver, string pattern )
        => receiver.MustRespectRegexNamingConvention( NamingConventionHelper.StarPatternToRegex( pattern ) );

    /// <summary>
    /// Reports a warning when any declaration in the selection fails  not respect the given naming convention, given as a regular expression.
    /// </summary>
    public static void MustRespectRegexNamingConvention( this IAspectReceiver<INamedDeclaration> receiver, string pattern )
    {
        var regex = new Regex( pattern );

        receiver.Where( d => !regex.IsMatch( d.Name ) )
            .ReportDiagnostic( d => ArchitectureDiagnosticDefinitions.NamingConventionViolation.WithArguments( (d, d.DeclarationKind, pattern) ) );
    }
}