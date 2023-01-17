// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.Architecture.Aspects;
using Metalama.Extensions.Architecture.Validators;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Extension methods that verify the architecture. These methods extend the <see cref="ArchitectureAmender"/> class, which is returned
    /// by the <see cref="AmenderExtensions.Verify(Metalama.Framework.Fabrics.IProjectAmender)"/> method of the <see cref="AmenderExtensions"/> class.
    /// </summary>
    [CompileTime]
    [PublicAPI]
    public static class ArchitectureAmenderExtensions
    {
        public static void Experimental( this ArchitectureAmender amender )
        {
            amender.WithTypes().AddAspect( _ => new ExperimentalAttribute() );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from a different context than the ones matching the specified <see cref="MatchingRule"/>. 
        /// </summary>
        public static void CanOnlyBeUsedFrom( this ArchitectureAmender amender, MatchingRule rule )
        {
            amender.WithTarget().ValidateReferences( new CanOnlyBeUsedFromValidator( rule, amender.Namespace ) );
        }

        /// <summary>
        /// Reports a warning when any type in the current scope is used from the context matching the specified <see cref="MatchingRule"/>.
        /// </summary>
        public static void CannotBeUsedFrom( this ArchitectureAmender amender, MatchingRule rule )
        {
            amender.WithTarget().ValidateReferences( new CannotBeUsedFromValidator( rule, amender.Namespace ) );
        }

        /// <summary>
        /// Reports a warning when any of the internal APIs of the current scope in used from a different context than the one allowed,
        /// except if this concept has access to the type using inheritance rules.
        /// </summary>
        public static void InternalsCanOnlyBeUsedFrom( this ArchitectureAmender amender, MatchingRule rule )
        {
            var internalValidator = new CanOnlyBeUsedFromValidator( rule, amender.Namespace );
            var nonInternalValidator = new InternalsCanOnlyBeUsedFromValidator( rule, amender.Namespace );

            amender.WithTypes().Where( t => t.Accessibility == Accessibility.Internal ).ValidateReferences( internalValidator );

            amender.WithTypes()
                .Where( t => t.Accessibility != Accessibility.Internal )
                .SelectMany(
                    t => t.Members()
                        .Where( m => m.Accessibility is Accessibility.Internal or Accessibility.PrivateProtected or Accessibility.ProtectedInternal ) )
                .ValidateReferences( nonInternalValidator );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a star pattern, i.e. where the <c>*</c> matches any sequence of characters, even empty.
        /// </summary>
        public static void DerivedTypesMustRespectNamingConvention( this ArchitectureAmender amender, string pattern )
        {
            amender.WithTarget().ValidateReferences( NamingConventionValidator.CreateStarPatternValidator( pattern ) );
        }

        /// <summary>
        /// Reports a warning when any type that inherits any type in the current scope does not follow a given convention, where the convention
        /// is given as a regular expression.
        /// </summary>
        public static void DerivedTypesMustRespectRegexNamingConvention( this ArchitectureAmender amender, string pattern )
        {
            amender.WithTarget().ValidateReferences( NamingConventionValidator.CreateRegexValidator( pattern ) );
        }

        public static ArchitectureAmender<IAssembly> WithReferencedAssembly( this ArchitectureAmender<ICompilation> amender, string assemblyName )
            => new( amender.WithTarget().SelectMany( c => c.ReferencedAssemblies.OfName( assemblyName ) ), a => a.Types );

        public static ArchitectureAmender<INamedType> WithTypes( this ArchitectureAmender<ICompilation> amender, IEnumerable<Type> types )
            => new( amender.WithTarget().SelectMany( _ => types.Select( t => (INamedType) TypeFactory.GetType( t ) ) ), t => new[] { t } );

        public static ArchitectureAmender<INamedType> WithTypes( this ArchitectureAmender<ICompilation> amender, params Type[] types )
            => amender.WithTypes( (IEnumerable<Type>) types );
    }
}