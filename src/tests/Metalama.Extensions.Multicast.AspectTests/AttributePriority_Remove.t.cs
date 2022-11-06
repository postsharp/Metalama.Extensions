[assembly: Metalama.Extensions.Multicast.AspectTests.AttributePriority_Remove.MyAspect("1", AttributePriority = 1)]
[assembly: Metalama.Extensions.Multicast.AspectTests.AttributePriority_Remove.MyAspect("2", AttributePriority = 2, AttributeExclude = true)]
public class C
{
  public void M()
  {
  }
}