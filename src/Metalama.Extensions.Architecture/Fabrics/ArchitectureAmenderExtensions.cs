﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Extension methods that verify the architecture. These methods extend the <see cref="ArchitectureAmender"/> class, which is returned
    /// by the <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/> method of the <see cref="AmenderExtensions"/> class.
    /// </summary>
    [CompileTime]
    public static class ArchitectureAmenderExtensions
    {
        public static void Experimental( this ArchitectureAmender amender )
        {
            amender.WithTypes( _ => true ).AddAspect( _ => new ExperimentalAttribute() );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from a different context than the ones matching the specified <see cref="UsageRule"/>. 
        /// </summary>
        public static void CanOnlyBeUsedFrom( this ArchitectureAmender amender, UsageRule rule, bool appliesToDerivedTypes = false )
        {
            amender.ValidatorReceiver.ValidateReferences( new CanOnlyBeUsedFromValidator( rule, amender.Namespace ) );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from the context matching the specified <see cref="UsageRule"/>.
        /// </summary>
        public static void CannotBeUsedFrom( this ArchitectureAmender amender, UsageRule rule, bool appliesToDerivedTypes = false )
        {
            amender.ValidatorReceiver.ValidateReferences( new CannotBeUsedFromValidator( rule, amender.Namespace ) );
        }

        /// <summary>
        /// Reports a warning when any of the internal APIs of the current scope in used from a different context than the one allowed,
        /// except if this concept has access to the type using inheritance rules.
        /// </summary>
        public static void InternalsCanOnlyBeUsedFrom( this ArchitectureAmender amender, UsageRule rule, bool appliesToDerivedTypes = false )
        {
            var internalValidator = new CanOnlyBeUsedFromValidator( rule, amender.Namespace );
            var nonInternalValidator = new CanOnlyBeUsedFromValidator( rule, amender.Namespace );

            amender.WithTypes( t => t.Accessibility == Accessibility.Internal ).ValidateReferences( internalValidator );
            amender.WithTypes( t => t.Accessibility != Accessibility.Internal ).ValidateReferences( nonInternalValidator );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a star pattern, i.e. where the <c>*</c> matches any sequence of characters, even empty.
        /// </summary>
        public static void DerivedTypesMustRespectNamingConvention( this ArchitectureAmender amender, string pattern )
        {
            amender.ValidatorReceiver.ValidateReferences( NamingConventionValidator.CreateStarPatternValidator( pattern ) );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a regular expression.
        /// </summary>
        public static void DerivedTypesMustRespectRegexNamingConvention( this ArchitectureAmender amender, string pattern )
        {
            amender.ValidatorReceiver.ValidateReferences( NamingConventionValidator.CreateRegexValidator( pattern ) );
        }
    }
}