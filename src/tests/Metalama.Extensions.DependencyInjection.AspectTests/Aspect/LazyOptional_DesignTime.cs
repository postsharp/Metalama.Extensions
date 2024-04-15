#if TEST_OPTIONS
// @DesignTime
// @ReportOutputWarnings
#endif

namespace Metalama.Extensions.DependencyInjection.AspectTests.Aspect.LazyOptional_DesignTime;

public partial class TargetClass
{
    [Dependency( IsLazy = true, IsRequired = false )]
    private readonly IFormatProvider _formatProvider;

    public TargetClass() { }

    public TargetClass( int x, IFormatProvider existingParameter ) { }
}