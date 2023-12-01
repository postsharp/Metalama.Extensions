public class TargetClass( IFormatProvider formatProvider )
{
    [Dependency]
    private readonly ILogger? _logger = (ILogger?) ServiceProviderProvider.ServiceProvider().GetService( typeof( ILogger ) );
    [Dependency]
    private IFormatProvider formatProvider = (IFormatProvider) ServiceProviderProvider.ServiceProvider().GetService( typeof( IFormatProvider ) ) ?? throw new InvalidOperationException( "The service 'IFormatProvider' could not be obtained from the service locator." );
}
