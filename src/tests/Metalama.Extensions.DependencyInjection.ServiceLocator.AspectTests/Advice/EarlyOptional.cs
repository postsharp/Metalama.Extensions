// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Extensions.Aspects;
using Metalama.Extensions.DependencyInjection;
using Metalama.Extensions.DependencyInjection.DotNet.Tests.Advice.EarlyOptional;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Extensions.DependencyInjection.DotNet.Tests.Advice.EarlyOptional;

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