﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Architecture.Fabrics;
using Metalama.Framework.Fabrics;
using System.IO;

namespace Metalama.Extensions.Architecture.AspectTests.NamingConvention.Legacy.Fabric_DerivedType;

#pragma warning disable CS0612, CS0618 // Type or member is obsolete

public class BadlyNamed : TextReader { }

internal class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Verify().SelectTypesDerivedFrom( typeof(TextReader) ).MustRespectNamingConvention( "*TextReader" );
    }
}