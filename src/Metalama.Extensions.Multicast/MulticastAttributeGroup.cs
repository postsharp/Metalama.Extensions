// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metalama.Extensions.Multicast;

[CompileTime]
internal class MulticastAttributeGroup
{
    private readonly MulticastTargets _allowedTargets;
    private readonly List<MulticastAttributeInfo> _attributes;

    public IAspectClass AspectClass { get; }

    public MulticastAttributeGroup( IAspectBuilder builder, MulticastTargets allowedTargets )
    {
        this._allowedTargets = allowedTargets;
        var aspectInstance = builder.AspectInstance;
        this._attributes = new List<MulticastAttributeInfo>( aspectInstance.SecondaryInstances.Length + 1 ) { new( aspectInstance, builder ) };
        this.AspectClass = builder.AspectInstance.AspectClass;

        foreach ( var instance in aspectInstance.SecondaryInstances )
        {
            this._attributes.Add( new MulticastAttributeInfo( instance, builder ) );
        }

        this._attributes.Sort();
    }

    public bool IsExcludeOnly => this._attributes.Count == 1 && this._attributes[0].Attribute.AttributeExclude;

    public bool IsMatch( IDeclaration declaration, MulticastTargets targets )
    {
        // Never match a declaration that already has a custom attribute for this aspect.
        if ( this.HasExcludeAttribute( declaration ) )
        {
            return false;
        }

        var isMatch = false;

        foreach ( var attribute in this._attributes )
        {
            if ( attribute.IsMatch( declaration, targets ) )
            {
                isMatch = !attribute.Attribute.AttributeExclude;
            }
        }

        return isMatch;
    }

    private bool HasExcludeAttribute( IDeclaration declaration )
    {
        var attributeOnTarget = declaration.Attributes.OfAttributeType( this.AspectClass.Type ).FirstOrDefault();

        if ( attributeOnTarget != null )
        {
            var attributeExcludeArgument = attributeOnTarget.NamedArguments.FirstOrDefault( a => a.Key == nameof(IMulticastAttribute.AttributeExclude) );

            if ( attributeExcludeArgument.Value.IsInitialized && (bool) attributeExcludeArgument.Value.Value! )
            {
                return true;
            }
        }

        return false;
    }

    public IAspect GetAspect( IDeclaration declaration, MulticastTargets targets )
    {
        for ( var i = this._attributes.Count - 1; i >= 0; i-- )
        {
            var attribute = this._attributes[i];

            if ( attribute.IsMatch( declaration, targets, true ) && !attribute.Attribute.AttributeExclude )
            {
                return attribute.Attribute;
            }
        }

        throw new InvalidOperationException( $"There is no matching aspect." );
    }

    public List<T> GetMatchingAspects<T>( IDeclaration declaration, MulticastTargets targets )
        where T : IAspect
    {
        var list = new List<T>();

        foreach ( var attribute in this._attributes )
        {
            if ( attribute.IsMatch( declaration, targets, true ) && attribute.Attribute is T aspect )
            {
                if ( attribute.Attribute.AttributeExclude )
                {
                    list.Clear();
                }
                else
                {
                    list.Add( aspect );
                }
            }
        }

        return list;
    }

    public bool TargetsAnyDeclarationKind( MulticastTargets targets )
    {
        if ( (targets & this._allowedTargets) == 0 )
        {
            return false;
        }

        foreach ( var attribute in this._attributes )
        {
            if ( attribute.Attribute.AttributeTargetElements == 0 || (attribute.Attribute.AttributeTargetElements & targets) != 0 )
            {
                return true;
            }
        }

        return false;
    }
}