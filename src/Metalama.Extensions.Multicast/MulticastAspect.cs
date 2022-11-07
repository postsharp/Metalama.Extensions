// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Serialization;
using System;

namespace Metalama.Extensions.Multicast;

public abstract class MulticastAspect : Aspect, IMulticastAttribute, IAspect<ICompilation>, IAspect<INamedType>
{
    internal static readonly DiagnosticDefinition<(string PropertyName, string AspectName, DeclarationKind DeclarationKind, IDeclaration Declaration, string
            Message)>
        InvalidRegexError
            = new( "PS0093", Severity.Error, "Invalid expression for property {0} of custom attribute {1} on {2} {3}: {4}", "Invalid regular expression." );

    private readonly MulticastTargets _targets;
    private readonly bool _multicastOnInheritance;

    [LamaNonSerialized]
    private MulticastImplementation? _implementation;

    protected MulticastAspect( MulticastTargets targets, bool multicastOnInheritance = false )
    {
        this._targets = targets;
        this._multicastOnInheritance = multicastOnInheritance;
    }

    protected MulticastAspect()
    {
        // TODO: remove.
    }

    public virtual void BuildEligibility( IEligibilityBuilder<ICompilation> builder ) { }

    public virtual void BuildAspect( IAspectBuilder<ICompilation> builder )
    {
        this.Implementation.AddAspects( builder );
    }

    public virtual void BuildEligibility( IEligibilityBuilder<INamedType> builder ) { }

    public virtual void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        this.Implementation.AddAspects( builder );
    }

    protected MulticastImplementation Implementation => this._implementation ??= new MulticastImplementation( this._targets, this._multicastOnInheritance );

    public MulticastTargets AttributeTargetElements { get; set; }

    [Obsolete]
    public string? AttributeTargetAssemblies { get; set; }

    public string? AttributeTargetTypes { get; set; }

    public MulticastAttributes AttributeTargetTypeAttributes { get; set; }

    [Obsolete]
    public MulticastAttributes AttributeTargetExternalTypeAttributes { get; set; }

    public string? AttributeTargetMembers { get; set; }

    public MulticastAttributes AttributeTargetMemberAttributes { get; set; }

    [Obsolete]
    public MulticastAttributes AttributeTargetExternalMemberAttributes { get; set; }

    public string? AttributeTargetParameters { get; set; }

    public MulticastAttributes AttributeTargetParameterAttributes { get; set; }

    public bool AttributeExclude { get; set; }

    public int AttributePriority { get; set; }

    [Obsolete]
    public bool AttributeReplace { get; set; }

    [Obsolete]
    public MulticastInheritance AttributeInheritance { get; set; }
}