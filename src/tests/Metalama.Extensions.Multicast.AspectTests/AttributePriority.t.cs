[assembly: Metalama.Extensions.Multicast.AspectTests.AttributePriority.MyAspect("1", AttributePriority = 1)]
[assembly: Metalama.Extensions.Multicast.AspectTests.AttributePriority.MyAspect("2", AttributePriority = 2)]
public class C
{
  public void M()
  {
    Console.WriteLine("Overridden: 2");
    return;
  }
}