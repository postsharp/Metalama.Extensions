[assembly: Metalama.Extensions.Multicast.AspectTests.HelloWorld.MyAspect("Hello, world.")]
public class C
{
  public void M()
  {
    Console.WriteLine("Overridden: Hello, world.");
    return;
  }
}