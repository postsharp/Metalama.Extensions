public class TargetClass
{
  [Dependency]
  private readonly ILogger? _logger;
  [Dependency]
  private IFormatProvider formatProvider;
  public TargetClass(IFormatProvider formatProvider)
  {
    this._logger = (ILogger? )ServiceProviderProvider.ServiceProvider().GetService(typeof(ILogger));
    this.formatProvider = (IFormatProvider)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider)) ?? throw new InvalidOperationException("The service 'IFormatProvider' could not be obtained from the service locator.");
  }
}