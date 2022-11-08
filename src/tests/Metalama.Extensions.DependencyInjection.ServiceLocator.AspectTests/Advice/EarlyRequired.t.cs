[MyAspect]
public class TargetClass
{
  public TargetClass()
  {
    this._formatProvider = (IFormatProvider)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider)) ?? throw new InvalidOperationException("The service 'IFormatProvider' could not be obtained from the service locator.");
  }
  public TargetClass(int x, IFormatProvider existingParameter)
  {
    this._formatProvider = (IFormatProvider)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider)) ?? throw new InvalidOperationException("The service 'IFormatProvider' could not be obtained from the service locator.");
  }
  private IFormatProvider _formatProvider;
}