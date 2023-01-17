// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Fabrics;
using System.Text.RegularExpressions;

#pragma warning disable CS0169

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.ExternalAssembly
{
    internal class Fabric : ProjectFabric
    {
        public override void AmendProject( IProjectAmender amender )
        {
            amender.Verify().WithReferencedAssembly( "System.Text.RegularExpressions" ).CannotBeUsedFrom( MatchingRule.NamespaceOfType( typeof( ForbiddenNamespace.ForbiddenClass ) ) );
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
