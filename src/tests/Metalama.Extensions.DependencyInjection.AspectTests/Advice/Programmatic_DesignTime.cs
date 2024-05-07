#if TEST_OPTIONS
// @DesignTime
// @ReportOutputWarnings
#endif

using Metalama.Framework.Aspects;
using Metalama.Extensions.DependencyInjection;
using Metalama.Extensions.DependencyInjection.AspectTests.Advice.Programmatic_DesignTime;
using Metalama.Framework.Code;

[assembly: AspectOrder( AspectOrderDirection.RunTime, typeof(DependencyAttribute), typeof(MyAspect) )]

namespace Metalama.Extensions.DependencyInjection.AspectTests.Advice.Programmatic_DesignTime;

public class MyAspect : TypeAspect
{
    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        builder.TryIntroduceDependency( new DependencyProperties( builder.Target, typeof(IFormatProvider), "_formatProvider" ), out _ );
    }
}

[MyAspect]
public partial class TargetClass
{
    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}