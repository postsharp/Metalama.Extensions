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
        public static ITypeSetVerifier<ICompilation> Verify( this IProjectAmender amender )
            => new CompilationSetVerifier( amender.Outbound, amender.Project.AssemblyName );

        /// <summary>
        /// Gets the architecture validation fluent API for a <see cref="NamespaceFabric"/>.
        /// </summary>
        public static ITypeSetVerifier<INamespace> Verify( this INamespaceAmender amender, bool includeChildNamespaces = true )
        {
            if ( includeChildNamespaces )
            {
                return new TypeSetVerifier<INamespace>(
                    amender.Outbound.SelectMany( ns => ns.DescendantsAndSelf() ),
                    x => x.SelectMany( ns => ns.DescendantsAndSelf().SelectMany( ns2 => ns2.Types ) ),
                    amender.Project.AssemblyName,
                    amender.Namespace );
            }
            else
            {
                return new TypeSetVerifier<INamespace>(
                    amender.Outbound,
                    x => x.SelectMany( ns => ns.Types ),
                    amender.Project.AssemblyName,
                    amender.Namespace );
            }
        }

        /// <summary>
        /// Gets the architecture validation fluent API for a <see cref="TypeFabric"/>.
        /// </summary>
        public static ITypeSetVerifier<INamedType> Verify( this ITypeAmender amender )
            => new TypeSetVerifier<INamedType>( amender.Outbound, x => x, amender.Project.AssemblyName, amender.Type.Namespace.FullName );
    }
}