// Warning CS0169 on `_formatProvider`: `The field 'TargetClass._formatProvider' is never used`
public partial class TargetClass
{
  [Dependency]
  private readonly IFormatProvider _formatProvider;
  public TargetClass()
  {
  }
  public TargetClass(int x, IFormatProvider existingParameter)
  {
  }
}