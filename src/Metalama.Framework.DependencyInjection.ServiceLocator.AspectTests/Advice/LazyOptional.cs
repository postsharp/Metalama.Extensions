// Copyright (c) SharpCrafters s.r.o.All rights reserved.
// This project is not open source.Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.DependencyInjection;
using Metalama.Framework.DependencyInjection.DotNet.Tests.Advice.LazyOptional;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Framework.DependencyInjection.DotNet.Tests.Advice.LazyOptional;

#pragma warning disable CS1591, CS8618

public class MyAspect : TypeAspect
{
    [IntroduceDependency( IsLazy = true, IsRequired = false )]
    private readonly IFormatProvider _formatProvider;
}

// <target>
[MyAspect]
public class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}