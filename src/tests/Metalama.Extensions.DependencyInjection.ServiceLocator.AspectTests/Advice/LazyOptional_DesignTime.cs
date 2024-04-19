﻿#if TEST_OPTIONS
// @DesignTime
// @ReportOutputWarnings
#endif

using Metalama.Extensions.DependencyInjection;
using Metalama.Extensions.DependencyInjection.DotNet.Tests.Advice.LazyOptional_DesignTime;
using Metalama.Framework.Aspects;

[assembly: AspectOrder( typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Extensions.DependencyInjection.DotNet.Tests.Advice.LazyOptional_DesignTime;

public class MyAspect : TypeAspect
{
    [IntroduceDependency( IsLazy = true, IsRequired = false )]
    private readonly IFormatProvider _formatProvider;
}

[MyAspect]
public partial class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}