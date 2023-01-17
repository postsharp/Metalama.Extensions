// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using System.Linq;

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Extension methods for <see cref="IProjectAmender"/>, <see cref="INamespaceAmender"/> and <see cref="ITypeAmender"/>. Entry point
    /// of the package when used in fabrics.
    /// </summary>
    [CompileTime]
    public static class AmenderExtensions
    {
        /// <summary>
        /// Gets the architecture validation fuent API for a <see cref="ProjectFabric"/>.
        /// </summary>
        public static ArchitectureVerifier<ICompilation> Verify( this IProjectAmender amender ) => new( amender.Outbound, c => c.Types );

        /// <summary>
        /// Gets the architecture validation fuent API for a <see cref="NamespaceFabric"/>.
        /// </summary>
        public static ArchitectureVerifier<INamespace> Verify( this INamespaceAmender amender, bool includeChildNamespaces = true )
        {
            if ( includeChildNamespaces )
            {
                return new ArchitectureVerifier<INamespace>(
                    amender.Outbound.SelectMany( ns => ns.DescendantsAndSelf() ),
                    ns => ns.DescendantsAndSelf().SelectMany( x => x.Types ),
                    amender.Namespace );
            }
            else
            {
                return new ArchitectureVerifier<INamespace>(
                    amender.Outbound,
                    ns => ns.Types,
                    amender.Namespace );
            }
        }

        /// <summary>
        /// Gets the architecture validation fuent API for a <see cref="TypeFabric"/>.
        /// </summary>
        public static ArchitectureVerifier<INamedType> Verify( this ITypeAmender amender )
            => new( amender.Outbound, type => new[] { type }, amender.Type.Namespace.FullName );
    }
}