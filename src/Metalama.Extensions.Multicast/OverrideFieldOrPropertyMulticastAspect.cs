// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System;
using System.Collections.Generic;

namespace Metalama.Extensions.Multicast;

[AttributeUsage(
    AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = true )]
public abstract class OverrideFieldOrPropertyMulticastAspect : MulticastAspect, IAspect<IFieldOrProperty>
{
    protected OverrideFieldOrPropertyMulticastAspect() : base( MulticastTargets.Field | MulticastTargets.Property ) { }

    public void BuildEligibility( IEligibilityBuilder<IFieldOrProperty> builder )
    {
        this.BuildEligibility( builder.DeclaringType() );

        builder.AddRule( EligibilityRuleFactory.GetAdviceEligibilityRule( AdviceKind.OverrideFieldOrProperty ) );
    }

    public void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
    {
        if ( this.Implementation.SkipIfExcluded( builder ) )
        {
            return;
        }

        var getterTemplateSelector = new GetterTemplateSelector(
            "get_" + nameof(this.OverrideProperty),
            "get_" + nameof(this.OverrideEnumerableProperty),
            "get_" + nameof(this.OverrideEnumeratorProperty) );

        builder.Advice.OverrideAccessors( builder.Target, getterTemplateSelector, "set_" + nameof(this.OverrideProperty) );
    }

    [Template]
    public abstract dynamic? OverrideProperty
    {
        get;
        set;
    }

    [Template( IsEmpty = true )]
    public virtual IEnumerable<dynamic?> OverrideEnumerableProperty => throw new NotSupportedException();

    [Template( IsEmpty = true )]
    public virtual IEnumerator<dynamic?> OverrideEnumeratorProperty => throw new NotSupportedException();
}