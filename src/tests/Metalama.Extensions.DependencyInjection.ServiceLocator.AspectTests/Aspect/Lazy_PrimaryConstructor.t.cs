public class TargetClass( IFormatProvider formatProvider )
{
    [Dependency( IsLazy = true )]
    private ILogger? _logger
    {
        get
        {
            return _loggerCache ??= (ILogger?) _serviceProvider!.GetService( typeof( ILogger ) );
        }
        init
        {
            throw new NotSupportedException( "Cannot set '_logger' because of the dependency aspect." );
        }
    }
    [Dependency( IsLazy = true )]
    private IFormatProvider formatProvider
    {
        get
        {
            return formatProviderCache ??= (IFormatProvider) _serviceProvider!.GetService( typeof( IFormatProvider ) );
        }
        set
        {
            throw new NotSupportedException( "Cannot set 'formatProvider' because of the dependency aspect." );
        }
    }
    private ILogger? _loggerCache;
    private IServiceProvider _serviceProvider = ServiceProviderProvider.ServiceProvider();
    private IFormatProvider? formatProviderCache;
}
