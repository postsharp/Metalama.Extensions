[assembly: Metalama.Extensions.Multicast.AspectTests.Replace_LeafNode.MyAspect("1")]
public class C
{
  [MyAspect("2")]
  public void M()
  {
    Console.WriteLine("Overridden: 2");
    return;
  }
}