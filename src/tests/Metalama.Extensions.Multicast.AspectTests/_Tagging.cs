// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Eligibility;
using System.Diagnostics;

namespace Metalama.Extensions.Multicast.AspectTests
{
    [AttributeUsage( AttributeTargets.All, AllowMultiple = true )]
    internal class AddTagAttribute : MulticastAspect, IAspect<IMethod>, IAspect<IConstructor>, IAspect<IFieldOrProperty>, IAspect<IEvent>, IAspect<IParameter>
    {
        private readonly string _tag;

        public AddTagAttribute( string tag ) :
            base( MulticastTargets.All )
        {
            this._tag = tag;
        }

        private void AddAttributes( IAspectBuilder<IDeclaration> builder )
        {
            Debugger.Break();

            foreach ( var aspect in this.Implementation.GetAspects<AddTagAttribute>( builder ) )
            {
                builder.Advice.IntroduceAttribute( builder.Target, AttributeConstruction.Create( typeof(TagAttribute), new[] { aspect._tag } ) );
            }
        }

        public void BuildAspect( IAspectBuilder<IMethod> builder )
        {
            this.AddAttributes( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IMethod> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IConstructor> builder )
        {
            this.AddAttributes( builder );
        }

        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            base.BuildAspect( builder );
            this.AddAttributes( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IConstructor> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
        {
            this.Implementation.AddAspects( builder );
            this.AddAttributes( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IFieldOrProperty> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IEvent> builder )
        {
            this.Implementation.AddAspects( builder );
            this.AddAttributes( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IEvent> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IParameter> builder )
        {
            this.AddAttributes( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IParameter> eligibility ) { }
    }

    [AttributeUsage( AttributeTargets.All, AllowMultiple = true )]
    [Inherited]
    internal class AddTagInheritedAttribute : MulticastAspect, IAspect<IMethod>, IAspect<IConstructor>
    {
        private readonly string _tag;

        public AddTagInheritedAttribute( string tag, bool multicastOnInheritance = false ) :
            base(
                MulticastTargets.Method | MulticastTargets.Parameter | MulticastTargets.Class | MulticastTargets.Struct
                | MulticastTargets.InstanceConstructor | MulticastTargets.StaticConstructor | MulticastTargets.Parameter
                | MulticastTargets.Property | MulticastTargets.Event,
                multicastOnInheritance )
        {
            this._tag = tag;
        }

        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            base.BuildAspect( builder );
            this.AddAttribute( builder );
        }

        private void AddAttribute( IAspectBuilder<IDeclaration> builder )
        {
            builder.Advice.IntroduceAttribute( builder.Target, AttributeConstruction.Create( typeof(TagAttribute), new[] { this._tag } ) );
        }

        public void BuildAspect( IAspectBuilder<IMethod> builder )
        {
            this.AddAttribute( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IMethod> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IConstructor> builder )
        {
            this.AddAttribute( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IConstructor> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IFieldOrProperty> builder )
        {
            this.AddAttribute( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IFieldOrProperty> eligibility ) { }

        public void BuildAspect( IAspectBuilder<IEvent> builder )
        {
            this.AddAttribute( builder );
        }

        public void BuildEligibility( IEligibilityBuilder<IEvent> eligibility ) { }
    }

    [AttributeUsage( AttributeTargets.All, AllowMultiple = true )]
    public class TagAttribute : Attribute
    {
        public TagAttribute( string tag ) { }
    }
}