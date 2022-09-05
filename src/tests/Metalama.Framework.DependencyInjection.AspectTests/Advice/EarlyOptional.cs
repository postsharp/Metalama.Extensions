﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.DependencyInjection;
using Metalama.Framework.DependencyInjection.Tests.Advice.EarlyOptional;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Framework.DependencyInjection.Tests.Advice.EarlyOptional;

#pragma warning disable CS1591

public class MyAspect : TypeAspect

{
    [IntroduceDependency( IsRequired = false )]
    private readonly IFormatProvider? _formatProvider;
}

// <target>
[MyAspect]
public class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}