﻿#if TEST_OPTIONS
// @DesignTime
// @ReportOutputWarnings
#endif

namespace Metalama.Extensions.DependencyInjection.DotNet.Tests.Aspect.LazyOptional_DesignTime;

// <target>
public partial class TargetClass
{
    [Dependency( IsLazy = true, IsRequired = false )]
    private readonly IFormatProvider _formatProvider;

    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}