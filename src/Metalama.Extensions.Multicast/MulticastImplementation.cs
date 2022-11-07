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
    private readonly bool _multicastOnInheritance;
    private IEligibilityRule<IDeclaration>? _eligibilityRule;

    public MulticastImplementation( MulticastTargets concreteTargets, bool multicastOnInheritance = false )
    {
        this._concreteTargets = concreteTargets;
        this._multicastOnInheritance = multicastOnInheritance;
        this._eligibilityRule = null;
    }

    public IEligibilityRule<IDeclaration> EligibilityRule => this._eligibilityRule ??= this.CreateEligibilityRule();

    private bool MustMulticast( IAspectBuilder<IDeclaration> builder )
        => this._multicastOnInheritance || builder.AspectInstance.Predecessors[0].Kind != AspectPredecessorKind.Inherited;

#pragma warning disable CA1822
    public bool SkipIfExcluded( IAspectBuilder<IDeclaration> builder )
#pragma warning restore CA1822
    {
        var attributeGroup = new MulticastAttributeGroup( builder, MulticastTargets.All );

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

    private static bool Filter( IDeclaration declaration, MulticastAttributeGroup attributeGroup, MulticastTargets targets )
        => attributeGroup.IsMatch( declaration, targets );

    private static bool FilterDeclaringType( INamedType type, MulticastAttributeGroup attributeGroup, MulticastTargets targets )
        => attributeGroup.IsMatch( type, targets ) && type.IsAspectEligible( attributeGroup.AspectClass.Type );

    private bool FilterType( INamedType type, MulticastAttributeGroup attributeGroup, MulticastTargets targets )
    {
        return this.MatchesTypeKind( type, targets ) && attributeGroup.IsMatch( type, targets );
    }

    private bool MatchesTypeKind( INamedType namedType, MulticastTargets targets )
    {
        var resultingTargets = targets == 0 ? this._concreteTargets : targets & this._concreteTargets;

        return namedType.TypeKind switch
        {
            TypeKind.Class or TypeKind.RecordClass => resultingTargets.HasFlagFast( MulticastTargets.Class ),
            TypeKind.Struct or TypeKind.RecordStruct => resultingTargets.HasFlagFast( MulticastTargets.Struct ),
            TypeKind.Interface => resultingTargets.HasFlagFast( MulticastTargets.Interface ),
            TypeKind.Delegate => resultingTargets.HasFlagFast( MulticastTargets.Delegate ),
            TypeKind.Enum => resultingTargets.HasFlagFast( MulticastTargets.Enum ),
            _ => false
        };
    }

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
        var attributeGroup = new MulticastAttributeGroup( builder, this._concreteTargets );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return;
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.AnyType ) )
        {
            builder
                .With( c => c.AllTypes.Where( t => implementation.FilterType( t, attributeGroup, MulticastTargets.AnyType ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.AnyType ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.StaticConstructor ) )
        {
            builder
                .With(
                    c => c.AllTypes.Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.StaticConstructor ) && t.StaticConstructor != null )
                        .Select( t => t.StaticConstructor! ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.StaticConstructor ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.InstanceConstructor ) )
        {
            builder
                .With(
                    compilation => compilation.AllTypes
                        .Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.InstanceConstructor ) )
                        .SelectMany( t => t.Constructors.Where( constructor => Filter( constructor, attributeGroup, MulticastTargets.InstanceConstructor ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.InstanceConstructor ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Method ) )
        {
            builder
                .With(
                    c => c.AllTypes
                        .Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.Method ) )
                        .SelectMany(
                            t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.Method ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Method ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    c => c.AllTypes
                        .Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.Method ) )
                        .SelectMany(
                            t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.Method ) ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup, MulticastTargets.Method ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Method ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.ReturnValue ) )
        {
            builder
                .With(
                    c => c.AllTypes
                        .Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.ReturnValue ) )
                        .SelectMany(
                            t => t.MethodsAndAccessors()
                                .Where(
                                    m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.ReturnValue )
                                                                 && !m.ReturnType.Is( SpecialType.Void ) ) )
                        .Select( m => m.ReturnParameter ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.ReturnValue ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Field ) )
        {
            builder
                .With(
                    c => c.AllTypes.Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.Field ) )
                        .SelectMany( t => t.Fields.Where( f => !f.IsImplicitlyDeclared && Filter( f, attributeGroup, MulticastTargets.Field ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Field ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Property ) )
        {
            builder
                .With(
                    c => c.AllTypes.Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.Property ) )
                        .SelectMany( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup, MulticastTargets.Property ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Property ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Event ) )
        {
            builder
                .With(
                    c => c.AllTypes.Where( t => FilterDeclaringType( t, attributeGroup, MulticastTargets.Event ) )
                        .SelectMany( t => t.Events.Where( e => !e.IsImplicitlyDeclared && Filter( e, attributeGroup, MulticastTargets.Event ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, e => attributeGroup.GetAspect( e, MulticastTargets.Event ) );
        }
    }

    public void AddAspects( IAspectBuilder<INamedType> builder )
    {
        if ( !this.MustMulticast( builder ) )
        {
            return;
        }

        // It seems that PostSharp did not multicast from a declaring type to nested types, so we don't implement it in Metalama.

        var attributeGroup = new MulticastAttributeGroup( builder, this._concreteTargets );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return;
        }

        // Multicast to children.
        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.StaticConstructor ) )
        {
            builder
                .With( t => t.StaticConstructor != null ? new[] { t.StaticConstructor } : Enumerable.Empty<IConstructor>() )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.StaticConstructor ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.InstanceConstructor ) )
        {
            builder
                .With( t => t.Constructors.Where( c => Filter( c, attributeGroup, MulticastTargets.InstanceConstructor ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.InstanceConstructor ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Method ) )
        {
            builder
                .With( t => t.MethodsAndAccessors().Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.Method ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Method ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    t => t.MethodsAndAccessors()
                        .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.Parameter ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup, MulticastTargets.Parameter ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Parameter ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.ReturnValue ) )
        {
            builder
                .With(
                    t => t.MethodsAndAccessors()
                        .Where(
                            m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.ReturnValue ) && !m.ReturnType.Is( SpecialType.Void ) )
                        .Select( m => m.ReturnParameter ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.ReturnValue ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Field ) )
        {
            builder
                .With( t => t.Fields.Where( f => !f.IsImplicitlyDeclared && Filter( f, attributeGroup, MulticastTargets.Field ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Field ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Property ) )
        {
            builder
                .With( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup, MulticastTargets.Property ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Property ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Event ) )
        {
            builder
                .With( t => t.Properties.Where( p => !p.IsImplicitlyDeclared && Filter( p, attributeGroup, MulticastTargets.Event ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Event ) );
        }
    }

    public void AddAspects( IAspectBuilder<IProperty> builder ) => this.AddAspectsToEventOrProperty( builder );

    private void AddAspectsToEventOrProperty( IAspectBuilder<IMemberWithAccessors> builder )
    {
        if ( !this.MustMulticast( builder ) )
        {
            return;
        }

        // It seems that PostSharp did not multicast from a declaring type to nested types, so we don't implement it in Metalama.

        var attributeGroup = new MulticastAttributeGroup( builder, this._concreteTargets );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return;
        }

        // Multicast to children.

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Method ) )
        {
            builder
                .With( t => t.Accessors.Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.Method ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Method ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Parameter ) )
        {
            builder
                .With(
                    t => t.Accessors
                        .Where( m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.Parameter ) )
                        .SelectMany( m => m.Parameters.Where( p => Filter( p, attributeGroup, MulticastTargets.Parameter ) ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Parameter ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.ReturnValue ) )
        {
            builder
                .With(
                    t => t.Accessors
                        .Where(
                            m => !m.IsImplicitlyDeclared && Filter( m, attributeGroup, MulticastTargets.ReturnValue ) && !m.ReturnType.Is( SpecialType.Void ) )
                        .Select( m => m.ReturnParameter ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.ReturnValue ) );
        }
    }

    public void AddAspects( IAspectBuilder<IEvent> builder ) => this.AddAspectsToEventOrProperty( builder );

    public void AddAspects( IAspectBuilder<IMethod> builder )
    {
        if ( !this.MustMulticast( builder ) )
        {
            return;
        }

        // It seems that PostSharp did not multicast from a declaring type to nested types, so we don't implement it in Metalama.

        var attributeGroup = new MulticastAttributeGroup( builder, this._concreteTargets );

        if ( attributeGroup.IsExcludeOnly )
        {
            builder.SkipAspect();

            return;
        }

        // Multicast to children.

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.Parameter ) )
        {
            builder
                .With( m => m.Parameters.Where( p => Filter( p, attributeGroup, MulticastTargets.Method ) ) )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.Method ) );
        }

        if ( attributeGroup.TargetsAnyDeclarationKind( MulticastTargets.ReturnValue ) )
        {
            builder
                .With( m => m.ReturnParameter.Type.Is( SpecialType.Void ) ? Enumerable.Empty<IParameter>() : new[] { m.ReturnParameter } )
                .AddAspectIfEligible( attributeGroup.AspectClass.Type, d => attributeGroup.GetAspect( d, MulticastTargets.ReturnValue ) );
        }
    }

    public void AddAspects( IAspectBuilder<IFieldOrProperty> builder )
    {
        switch ( builder )
        {
            case IAspectBuilder<IField>:
                // There is nothing to do because we cannot multicast from a field.
                break;

            case IAspectBuilder<IProperty> propertyAspectBuilder:
                this.AddAspects( propertyAspectBuilder );

                break;

            default:
                throw new ArgumentOutOfRangeException( nameof(builder) );
        }
    }

    public List<T> GetAspects<T>( IAspectBuilder<IDeclaration> builder )
        where T : IAspect
    {
        var attributeGroup = new MulticastAttributeGroup( builder, this._concreteTargets );

        return attributeGroup.GetMatchingAspects<T>( builder.Target, MulticastTargetsHelper.GetMulticastTargets( builder.Target ) );
    }
}