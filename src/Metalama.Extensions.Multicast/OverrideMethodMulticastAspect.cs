﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metalama.Extensions.Multicast;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Event,
    AllowMultiple = true )]
public abstract class OverrideMethodMulticastAspect : MulticastAspect, IAspect<IProperty>,
                                                      IAspect<IEvent>, IAspect<IMethod>
{
    protected OverrideMethodMulticastAspect() : base( MulticastTargets.Method ) { }

    public virtual void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        if ( this.Implementation.SkipIfExcluded( builder ) )
        {
            return;
        }

        if ( !builder.VerifyEligibility( this.Implementation.EligibilityRule ) )
        {
            return;
        }

#if NET5_0_OR_GREATER
        var templates = new MethodTemplateSelector(
            nameof(this.OverrideMethod),
            nameof(this.OverrideAsyncMethod),
            nameof(this.OverrideEnumerableMethod),
            nameof(this.OverrideEnumeratorMethod),
            nameof(this.OverrideAsyncEnumerableMethod),
            nameof(this.OverrideAsyncEnumeratorMethod),
            this.UseAsyncTemplateForAnyAwaitable,
            this.UseEnumerableTemplateForAnyEnumerable );
#else
        var templates = new MethodTemplateSelector(
            nameof( this.OverrideMethod ),
            nameof( this.OverrideAsyncMethod ),
            nameof( this.OverrideEnumerableMethod ),
            nameof( this.OverrideEnumeratorMethod ),
            null,
            null,
            this.UseAsyncTemplateForAnyAwaitable,
            this.UseEnumerableTemplateForAnyEnumerable );
#endif

        builder.Advice.Override( builder.Target, templates );
    }

    public virtual void BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        builder.MustBeExplicitlyDeclared();
        this.BuildEligibility( builder.DeclaringType() );
    }

    public virtual void BuildAspect( IAspectBuilder<IProperty> builder )
    {
        this.Implementation.AddAspects( builder );
    }

    public virtual void BuildEligibility( IEligibilityBuilder<IEvent> builder )
    {
        this.BuildEligibility( builder.DeclaringType() );
    }

    public virtual void BuildAspect( IAspectBuilder<IEvent> builder )
    {
        this.Implementation.AddAspects( builder );
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="OverrideAsyncMethod"/> template must be applied to all methods returning an awaitable
    /// type (including <c>IAsyncEnumerable</c> and <c>IAsyncEnumerator</c>), instead of only to methods that have the <c>async</c> modifier.
    /// </summary>
    protected bool UseEnumerableTemplateForAnyEnumerable { get; init; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="OverrideEnumerableMethod"/>, <see cref="OverrideEnumeratorMethod"/>,
    /// <c>OverrideAsyncEnumerableMethod"</c> or  <c>OverrideAsyncEnumeratorMethod"</c> template must be applied to all methods returning
    /// a compatible return type, instead of only to methods using the <c>yield</c> statement.
    /// </summary>
    protected bool UseAsyncTemplateForAnyAwaitable { get; init; }
#pragma warning restore SA1623

    public virtual void BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        this.BuildEligibility( builder.DeclaringType() );
        builder.AddRule( EligibilityRuleFactory.GetAdviceEligibilityRule( AdviceKind.OverrideMethod ) );
    }

    [Template( IsEmpty = true )]
    public virtual Task<dynamic?> OverrideAsyncMethod() => throw new NotSupportedException();

    [Template( IsEmpty = true )]
    public virtual IEnumerable<dynamic?> OverrideEnumerableMethod() => throw new NotSupportedException();

    [Template( IsEmpty = true )]
    public virtual IEnumerator<dynamic?> OverrideEnumeratorMethod() => throw new NotSupportedException();

#if NET5_0_OR_GREATER
    [Template( IsEmpty = true )]
    public virtual IAsyncEnumerable<dynamic?> OverrideAsyncEnumerableMethod() => throw new NotSupportedException();

    [Template( IsEmpty = true )]
    public virtual IAsyncEnumerator<dynamic?> OverrideAsyncEnumeratorMethod() => throw new NotSupportedException();
#endif

    /// <summary>
    /// Default template of the new method implementation.
    /// </summary>
    /// <returns></returns>
    [Template]
    public abstract dynamic? OverrideMethod();
}