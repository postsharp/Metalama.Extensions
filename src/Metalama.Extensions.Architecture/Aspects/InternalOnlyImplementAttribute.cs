// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Validation;

namespace Metalama.Extensions.Architecture.Aspects
{
    /// <summary>
    /// Aspect that, when applied to an interface, reports a warning whenever a type attempts to implement this interface,
    /// unless the type is inside the same project as the target interface, or the project has access to internal members of the project
    /// defining the target interface.
    /// </summary>
    [PublicAPI]
    public class InternalOnlyImplementAttribute : TypeAspect
    {
        public override void BuildAspect( IAspectBuilder<INamedType> builder )
        {
            builder.Outbound.ValidateReferences( this.ValidateReference, ReferenceKinds.BaseType );
        }

        private void ValidateReference( in ReferenceValidationContext context )
        {
            if ( context.ReferencedDeclaration.DeclaringAssembly.AreInternalsVisibleFrom( context.ReferencingDeclaration.DeclaringAssembly ) )
            {
                return;
            }

            context.Diagnostics.Report(
                ArchitectureDiagnosticDefinitions.InternalImplement.WithArguments(
                    (context.ReferencedDeclaration, context.ReferencedDeclaration.DeclaringAssembly.Identity.Name) ) );
        }

        public override void BuildEligibility( IEligibilityBuilder<INamedType> builder )
            => builder.MustSatisfy( type => type.TypeKind == TypeKind.Interface, t => $"{t} must be an interface" );
    }
}