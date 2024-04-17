// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.ExternalType.ForbiddenNamespace;
using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Fabrics;
using System.Text.RegularExpressions;

#pragma warning disable CS0169, CS0618

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.Legacy.ExternalType
{
    internal class Fabric : ProjectFabric
    {
        public override void AmendProject( IProjectAmender amender )
        {
#pragma warning disable CS0612 // Type or member is obsolete
            amender.Verify().SelectTypes( typeof(Regex) ).CannotBeUsedFrom( r => r.NamespaceOf( typeof(ForbiddenClass) ) );
#pragma warning restore CS0612 // Type or member is obsolete
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