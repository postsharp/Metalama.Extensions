[assembly: Metalama.Extensions.Multicast.AspectTests.Exclude_LeafNode.MyAspect("1")]
public class C
{
  [MyAspect("2", AttributeExclude = true)]
  public void M()
  {
  }
}