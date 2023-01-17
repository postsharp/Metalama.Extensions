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
        public static ArchitectureAmender<ICompilation> Verify( this IProjectAmender amender ) => new( amender.Outbound, c => c.Types );

        public static ArchitectureAmender<INamespace> Verify( this INamespaceAmender amender, bool includeChildNamespaces = true )
            => new( amender.Outbound, includeChildNamespaces ? ns => ns.DescendantsAndSelf().SelectMany( x => x.Types ) : ns => ns.Types );

        public static ArchitectureAmender<INamedType> Verify( this ITypeAmender amender ) => new( amender.Outbound, type => new[] { type } );
    }
}