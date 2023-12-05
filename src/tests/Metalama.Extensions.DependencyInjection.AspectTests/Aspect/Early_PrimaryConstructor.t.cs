public class TargetClass( IFormatProvider formatProvider, ILogger? logger = default )
{
    [Dependency]
    private readonly ILogger _logger = logger ?? throw new System.ArgumentNullException( nameof( logger ) );
    [Dependency]
    private IFormatProvider formatProvider = formatProvider ?? throw new System.ArgumentNullException( nameof( formatProvider ) );
}
