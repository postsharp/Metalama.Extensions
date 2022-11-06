[assembly: Metalama.Extensions.Multicast.AspectTests.Replace_IntermediateNode.MyAspect("1")]
[MyAspect("2")]
public class C
{
  public void M()
  {
    Console.WriteLine("Overridden: 2");
    return;
  }
}