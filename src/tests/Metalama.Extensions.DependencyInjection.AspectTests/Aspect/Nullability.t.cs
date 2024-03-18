using Microsoft.Extensions.Logging;
namespace Metalama.Extensions.DependencyInjection.AspectTests.Aspect.Nullability;
public class C
{
  // Optional.
  [Dependency]
  private ILoggerFactory? _loggerFactory;
  // Required.
  [Dependency]
  private IFormatProvider _formatProvider;
  public C(ILoggerFactory? loggerFactory = default, IFormatProvider? formatProvider = default)
  {
    this._loggerFactory = loggerFactory;
    this._formatProvider = formatProvider ?? throw new System.ArgumentNullException(nameof(formatProvider));
  }
}