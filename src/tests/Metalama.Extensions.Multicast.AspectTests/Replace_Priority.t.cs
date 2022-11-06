[assembly: Metalama.Extensions.Multicast.AspectTests.Replace_Priority.MyAspect("1", AttributePriority = 1)]
[assembly: Metalama.Extensions.Multicast.AspectTests.Replace_Priority.MyAspect("2", AttributePriority = 2)]
public class C
{
  public void M()
  {
    Console.WriteLine("Overridden: 2");
    return;
  }
}