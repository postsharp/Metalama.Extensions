// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.ExternalAssembly.ForbiddenNamespace;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;
using System.Text.RegularExpressions;

#pragma warning disable CS0169
#pragma warning disable CS0612 , CS0618 // Type or member is obsolete

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.ExternalAssembly
{
    internal class Fabric : ProjectFabric
    {
        public override void AmendProject( IProjectAmender amender )
        {
            amender.Verify().WithReferencedAssembly( "System.Text.RegularExpressions" ).CannotBeUsedFrom( r => r.NamespaceOf( typeof(ForbiddenClass) ) );
        }
    }

    namespace ForbiddenNamespace
    {
        internal class ForbiddenClass
        {
            private readonly Regex? _regex;
        }
    }

    internal class AllowedClass
    {
        private readonly Regex? _regex;
    }
}