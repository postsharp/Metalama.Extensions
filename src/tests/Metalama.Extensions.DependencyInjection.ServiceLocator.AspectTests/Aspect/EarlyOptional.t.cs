public class TargetClass
{
  [Dependency(IsRequired = false)]
  private readonly IFormatProvider? _formatProvider;
  public TargetClass()
  {
    this._formatProvider = (IFormatProvider? )ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider));
  }
  public TargetClass(int x, IFormatProvider existingParameter)
  {
    this._formatProvider = (IFormatProvider? )ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider));
  }
}