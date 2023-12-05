public struct TargetStruct( Func<ILogger>? logger = default )
{
    [Dependency( IsLazy = true )]
    private ILogger _logger
    {
        get
        {
            return _loggerCache ??= _loggerFunc!.Invoke();
        }
        init
        {
            throw new NotSupportedException( "Cannot set '_logger' because of the dependency aspect." );
        }
    }
    private ILogger? _loggerCache = default;
    private Func<ILogger> _loggerFunc = logger ?? throw new System.ArgumentNullException( nameof( logger ) );
}
