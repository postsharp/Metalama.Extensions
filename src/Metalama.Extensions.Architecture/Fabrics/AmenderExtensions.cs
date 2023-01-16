// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture.Fabrics
{
    /// <summary>
    /// Extension methods for <see cref="IProjectAmender"/>, <see cref="INamespaceAmender"/> and <see cref="ITypeAmender"/>. Entry point
    /// of the package when used in fabrics.
    /// </summary>
    [CompileTime]
    public static class AmenderExtensions
    {
        public static ProjectArchitectureAmender Verify( this IProjectAmender amender ) => new( amender );

        public static NamespaceArchitectureAmender Verify( this INamespaceAmender amender, bool includeChildNamespaces = true )
            => new( amender, includeChildNamespaces );

        public static TypeArchitectureAmender Verify( this ITypeAmender amender ) => new( amender );
    }
}