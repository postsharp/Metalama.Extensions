[MyAspect]
public class TargetClass
{
    public TargetClass()
    {
        this._formatProvider = (IFormatProvider?)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider));
    }

    public TargetClass(int x, IFormatProvider existingParameter)
    {
        this._formatProvider = (IFormatProvider?)ServiceProviderProvider.ServiceProvider().GetService(typeof(IFormatProvider));
    }

    private IFormatProvider? _formatProvider;
}