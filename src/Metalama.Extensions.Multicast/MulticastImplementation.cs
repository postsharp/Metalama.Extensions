// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System;
using System.Collections.Generic;
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

    private static bool Filter( IDeclaration declaration, MulticastAttributeGroup attributeGroup )
        => attributeGroup.IsMatch( declaration ) && declaration.IsEligible( attributeGroup.AspectClass.Type );

    private bool FilterType( INamedType type, MulticastAttributeGroup attributeGroup )
        => this.MatchesTypeKind( type ) && attributeGroup.IsMatch( type ) && type.IsEligible( attributeGroup.AspectClass.Type );

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
            rules.Add( builder => builder.MustBe<ICompilation>() );
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
            rules.Add( builder => builder.MustBe<IMethod>() );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.InstanceConstructor ) )
        {
            rules.Add( builder => builder.Convert().To<IConstructor>().MustBeNonStatic() );
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
                    builder.MustBe<IProperty>();
                } );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Event ) )
        {
            rules.Add( builder => builder.MustBe<IEvent>() );
        }

        if ( this._concreteTargets.HasFlagFast( MulticastTargets.Field ) )
        {
            rules.Add(
                builder =>
                {
                    builder.MustBeExplicitlyDeclared();
                    builder.MustBe<IEvent>();
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
                .With( c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.StaticConstructor ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) && t.StaticConstructor != null )
                        .Select( t => t.StaticConstructor! ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.InstanceConstructor ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Constructors.Where( c => Filter( c, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Method ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    c => c.Types
                        .Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
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
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Field ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Fields.Where( f => !f.IsImplicitlyDeclared && Filter( f, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Property ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Event ) )
        {
            builder
                .With(
                    c => c.Types.Where( t => implementation.FilterType( t, attributeGroup ) )
                        .SelectMany( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
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
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.InstanceConstructor ) )
        {
            builder
                .With( t => t.Constructors.Where( c => Filter( c, attributeGroup ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Method ) )
        {
            builder
                .With( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    t => t.MethodsAndAccessors()
                        .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup ) ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.ReturnValue ) )
        {
            builder
                .With(
                    t => t.MethodsAndAccessors()
                        .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup ) && !m.ReturnType.Is( SpecialType.Void ) )
                        .Select( m => m.ReturnParameter ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Field ) )
        {
            builder
                .With( t => t.Fields.Where( f => !f.IsImplicitlyDeclared && Filter( f, attributeGroup ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Property ) )
        {
            builder
                .With( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }

        if ( implementation._concreteTargets.HasFlagFast( MulticastTargets.Event ) )
        {
            builder
                .With( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup ) ) )
                .AddAspect( attributeGroup.AspectClass.Type, _ => attributeGroup.Aspect );
        }
    }

    public void AddAspects( IAspectBuilder<IProperty> builder ) => throw new NotImplementedException();

    public void AddAspects( IAspectBuilder<IEvent> builder ) => throw new NotImplementedException();

    public void AddAspects( IAspectBuilder<IMethod> builder ) => throw new NotImplementedException();
}