// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using System.IO;

namespace Metalama.Extensions.Architecture.AspectTests.NamingConvention.Fabric_DerivedType;

public class BadlyNamed : TextReader { }

internal class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.SelectTypesDerivedFrom( typeof(TextReader) ).MustRespectNamingConvention( "*TextReader" );
    }
}