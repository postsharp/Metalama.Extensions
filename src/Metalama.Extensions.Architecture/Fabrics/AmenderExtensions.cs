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
        /// Gets the architecture validation fluent API for a <see cref="ProjectFabric"/>.
        /// </summary>
        public static ITypeArchitectureVerifier<ICompilation> Verify( this IProjectAmender amender )
            => new TypeArchitectureVerifier<ICompilation>( amender.Outbound, x => x.SelectMany( c => c.Types ) );

        /// <summary>
        /// Gets the architecture validation fluent API for a <see cref="NamespaceFabric"/>.
        /// </summary>
        public static ITypeArchitectureVerifier<INamespace> Verify( this INamespaceAmender amender, bool includeChildNamespaces = true )
        {
            if ( includeChildNamespaces )
            {
                return new TypeArchitectureVerifier<INamespace>(
                    amender.Outbound.SelectMany( ns => ns.DescendantsAndSelf() ),
                    x => x.SelectMany( ns => ns.DescendantsAndSelf().SelectMany( x => x.Types ) ),
                    amender.Namespace );
            }
            else
            {
                return new TypeArchitectureVerifier<INamespace>(
                    amender.Outbound,
                    x => x.SelectMany( ns => ns.Types ),
                    amender.Namespace );
            }
        }

        /// <summary>
        /// Gets the architecture validation fluent API for a <see cref="TypeFabric"/>.
        /// </summary>
        public static ITypeArchitectureVerifier<INamedType> Verify( this ITypeAmender amender )
            => new TypeArchitectureVerifier<INamedType>( amender.Outbound, x => x, amender.Type.Namespace.FullName );
    }
}