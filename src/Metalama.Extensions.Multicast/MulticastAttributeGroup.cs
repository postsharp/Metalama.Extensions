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
    private readonly List<MulticastAttributeInfo> _attributes;

    public IAspectClass AspectClass { get; }

    public MulticastAttributeGroup( IAspectBuilder builder )
    {
        this._attributes = new List<MulticastAttributeInfo>( builder.AspectInstance.SecondaryInstances.Length + 1 );
        this._attributes.Add( new MulticastAttributeInfo( builder.AspectInstance, builder ) );
        this.AspectClass = builder.AspectInstance.AspectClass;

        foreach ( var instance in builder.AspectInstance.SecondaryInstances )
        {
            this._attributes.Add( new MulticastAttributeInfo( instance, builder ) );
        }

        this._attributes.Sort();
    }

    public bool IsExcludeOnly => this._attributes.Count == 0 && this._attributes[0].Attribute.AttributeExclude;

    public bool IsMatch( IDeclaration declaration )
    {
        // Never match a declaration that already has a custom attribute for this aspect.
        if ( this.HasExcludeAttribute( declaration ) )
        {
            return false;
        }

        var isMatch = false;

        foreach ( var attribute in this._attributes )
        {
            if ( attribute.IsMatch( declaration ) )
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

    public IAspect Aspect => this._attributes[this._attributes.Count - 1].Attribute;
}