// Copyright (c) SharpCrafters s.r.o. All rights reserved. See LICENSE.md in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.DependencyInjection;
using Metalama.Framework.DependencyInjection.DotNet.Tests.Advice.LazyRequired;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Framework.DependencyInjection.DotNet.Tests.Advice.LazyRequired;

#pragma warning disable CS1591, CS8618

public class MyAspect : TypeAspect
{
    [IntroduceDependency( IsLazy = true )]
    private readonly IFormatProvider _formatProvider;
}

// <target>
[MyAspect]
public class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}