// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Metalama.Extensions.Multicast;

[CompileTime]
public sealed class MulticastImplementation : IMulticastImplementation<ICompilation>, IMulticastImplementation<INamedType>,
                                              IMulticastImplementation<IProperty>,
                                              IMulticastImplementation<IEvent>, IMulticastImplementation<IMethod>
{
    private readonly MulticastTargets _concreteTargets;
    private IEligibilityRule<IDeclaration>? _eligibilityRule;

    public MulticastImplementation( MulticastTargets concreteTargets )
    {
        this._concreteTargets = concreteTargets;
        this._eligibilityRule = null;
    }

    private static bool Filter( IDeclaration declaration, MulticastAttributeGroup attributeGroup ) => attributeGroup.IsMatch( declaration );

    private bool FilterType( INamedType type, MulticastAttributeGroup attributeGroup, bool forMembers = true )
    {
        // When this method is called to filter the declaring type, we must not test the type kind.
        // When this method is called to test the final, target type, it is redundant to call IsAspectEligible because it will be done by AddAspectIfEligible.
        return (forMembers || this.MatchesTypeKind( type )) && attributeGroup.IsMatch( type )
                                                            && (!forMembers || type.IsAspectEligible( attributeGroup.AspectClass.Type ));
    }

    private bool MatchesTypeKind( INamedType namedType )
        => namedType.TypeKind switch
        {
            TypeKind.Class or TypeKind.RecordClass => this._concreteTargets.HasFlagFast( MulticastTargets.Class ),
            TypeKind.Struct or TypeKind.RecordStruct => this._concreteTargets.HasFlagFast( MulticastTargets.Struct ),
            TypeKind.Interface => this._concreteTargets.HasFlagFast( MulticastTargets.Interface ),
            _ => false
        };

    public IEligibilityRule<IDeclaration> EligibilityRule => this._eligibilityRule ??= this.CreateEligibilityRule();

    private IEligibilityRule<IDeclaration> CreateEligibilityRule()
    {
        List<Action<IEligibilityBuilder<IDeclaration>>> rules = new();

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Assembly ) )
        {
            rules.Add( builder => builder.MustBeOfType( typeof(ICompilation) ) );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Class ) )
        {
            rules.Add(
                builder => builder.Convert()
                    .To<INamedType>()
                    .MustSatisfy( t => t.TypeKind is TypeKind.RecordClass or TypeKind.Class, t => $"{t} is not a class or record class" ) );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Struct ) )
        {
            rules.Add(
                builder => builder.Convert()
                    .To<INamedType>()
                    .MustSatisfy( t => t.TypeKind is TypeKind.RecordStruct or TypeKind.Struct, t => $"{t} is not a struct or record struct" ) );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Method ) )
        {
            rules.Add( builder => builder.MustBeOfType( typeof(IMethod) ) );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.InstanceConstructor ) )
        {
            rules.Add( builder => builder.Convert().To<IConstructor>().MustNotBeStatic() );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.StaticConstructor ) )
        {
            rules.Add( builder => builder.Convert().To<IConstructor>().MustBeStatic() );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Property ) )
        {
            rules.Add(
                builder =>
                {
                    builder.MustBeExplicitlyDeclared();
                    builder.MustBeOfType( typeof(IProperty) );
                } );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Event ) )
        {
            rules.Add( builder => builder.MustBeOfType( typeof(IEvent) ) );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Field ) )
        {
            rules.Add(
                builder =>
                {
                    builder.MustBeExplicitlyDeclared();
                    builder.MustBeOfType( typeof(IField) );
                } );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Parameter ) )
        {
            rules.Add(
                builder =>
                {
                    var parameterEligibility = builder.Convert().To<IParameter>();
                    parameterEligibility.DeclaringMember().MustBeExplicitlyDeclared();
                    parameterEligibility.MustSatisfy( p => !p.IsReturnParameter, p => $"{p} is the return parameter" );
                } );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.ReturnValue ) )
        {
            rules.Add(
                builder =>
                {
                    var parameterEligibility = builder.Convert().To<IParameter>();
                    parameterEligibility.DeclaringMember().MustBeExplicitlyDeclared();
                    parameterEligibility.MustSatisfy( p => !p.IsReturnParameter, p => $"{p} is not the return parameter" );
                } );
        }

        return EligibilityRuleFactory.CreateRule<IDeclaration>( builder => builder.MustSatisfyAny( rules.ToArray() ) );
    }

    public void AddAspects( IAspectBuilder<ICompilation> builder )
    {
        var implementation = this;
        var attributeGroup = new MulticastAttributeGroup( builder );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return;
        }

        if ( implementation._concreteTargets.HasAnyFlag( MulticastTargets.Class | MulticastTargets.Struct | MulticastTargets.Interface ) )
        {
            builder
                .With( c => c.Types.Where( t => implementation.FilterType( t, attributeGroup, false ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.StaticConstructor ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) && t.StaticConstructor != null )
                        .Select( t => t.StaticConstructor! ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.InstanceConstructor ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Constructors.Where( c => Filter( c, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Method ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.ReturnValue ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany(
                            t => t.MethodsAndAccessors()
                                .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) && !m.ReturnType.Is( SpecialType.Void ) ) )
                        .Select( m => m.ReturnParameter ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Field ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Fields.Where( f => !f.IsImplicitlyDeclared && Filter( f, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Property ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Event ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }
    }

    public void AddAspects( IAspectBuilder<INamedType> builder )
    {
        var implementation = this;
        var attributeGroup = new MulticastAttributeGroup( builder );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return;
        }

        // Multicast to children.
        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.StaticConstructor ) )
        {
            builder
                .With( t => t.StaticConstructor != null ? new[] { t.StaticConstructor } : Enumerable.Empty<IConstructor>() )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.InstanceConstructor ) )
        {
            builder
                .With( t => t.Constructors.Where( c => Filter( c, attributeGroup ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Method ) )
        {
            builder
                .With( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    t => t.MethodsAndAccessors()
                        .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.ReturnValue ) )
        {
            builder
                .With(
                    t => t.MethodsAndAccessors()
                        .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) && !m.ReturnType.Is( SpecialType.Void ) )
                        .Select( m => m.ReturnParameter ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Field ) )
        {
            builder
                .With( t => t.Fields.Where( f => !f.IsImplicitlyDeclared && Filter( f, attributeGroup ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Property ) )
        {
            builder
                .With( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Event ) )
        {
            builder
                .With( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d ) );
        }
    }

    public bool SkipIfExcluded( IAspectBuilder<IDeclaration> builder )
    {
        var attributeGroup = new MulticastAttributeGroup( builder );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddAspects( IAspectBuilder<IProperty> builder ) => throw new NotImplementedException();

    public void AddAspects( IAspectBuilder<IEvent> builder ) => throw new NotImplementedException();

    public void AddAspects( IAspectBuilder<IMethod> builder ) => throw new NotImplementedException();
}