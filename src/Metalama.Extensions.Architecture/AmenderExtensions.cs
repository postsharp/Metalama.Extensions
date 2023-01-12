// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Fabrics;

namespace Metalama.Extensions.Architecture
{
    /// <summary>
    /// Extension methods for <see cref="IProjectAmender"/>, <see cref="INamespaceAmender"/> and <see cref="ITypeAmender"/>. Entry point
    /// of the package when used in fabrics.
    /// </summary>
    [CompileTime]
    public static class AmenderExtensions
    {
        public static ArchitectureAmender Verify( this IProjectAmender amender ) => new ProjectArchitectureAmender( amender );

        public static ArchitectureAmender Verify( this INamespaceAmender amender, bool includeChildNamespaces = true )
            => new NamespaceArchitectureAmender( amender, includeChildNamespaces );

        public static ArchitectureAmender Verify( this ITypeAmender amender ) => new TypeArchitectureAmender( amender );
    }
}