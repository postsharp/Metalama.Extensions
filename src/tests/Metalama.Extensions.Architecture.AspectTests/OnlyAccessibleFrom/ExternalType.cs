// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.ExternalType.ForbiddenNamespace;
using Metalama.Extensions.Architecture.Predicates;
using Metalama.Framework.Aspects;
using Metalama.Framework.Fabrics;
using System.Text.RegularExpressions;

#pragma warning disable CS0169

namespace Metalama.Extensions.Architecture.AspectTests.OnlyAccessibleFrom.ExternalType
{
    internal class Fabric : ProjectFabric
    {
        public override void AmendProject( IProjectAmender amender )
        {
            amender.SelectReflectionType( typeof(Regex) ).CannotBeUsedFrom( r => r.NamespaceOf( typeof(ForbiddenClass) ) );
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