public class TargetClass
{
  [Dependency]
  private readonly IFormatProvider _formatProvider;
  public TargetClass()
  {
    _formatProvider = (IFormatProvider)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider)) ?? throw new InvalidOperationException("The service 'IFormatProvider' could not be obtained from the service locator.");
  }
  public TargetClass(int x, IFormatProvider existingParameter)
  {
    _formatProvider = (IFormatProvider)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider)) ?? throw new InvalidOperationException("The service 'IFormatProvider' could not be obtained from the service locator.");
  }
}