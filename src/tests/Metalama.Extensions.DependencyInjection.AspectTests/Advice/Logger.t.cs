public class Foo
{
  [TheAspect]
  public void Bar()
  {
    LoggerExtensions.LogTrace(this._logger, "Starting Foo.Bar()");
  }
  [TheAspect]
  public void Bar2()
  {
    LoggerExtensions.LogTrace(this._logger, "Starting Foo.Bar2()");
  }
  private ILogger _logger;
  public Foo(ILogger<Foo> logger = default)
  {
    this._logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
  }
}