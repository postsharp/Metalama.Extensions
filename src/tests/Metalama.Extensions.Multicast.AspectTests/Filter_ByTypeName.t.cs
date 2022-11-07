[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "C", AttributeTargetElements = MulticastTargets.Class, AttributeTargetTypes = "Metalama.Extensions.Multicast.AspectTests.Filter_ByTypeName.C" )]
[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "C+N", AttributeTargetElements = MulticastTargets.Class, AttributeTargetTypes = "Metalama.Extensions.Multicast.AspectTests.Filter_ByTypeName.C+N" )]
[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "C<T>", AttributeTargetElements = MulticastTargets.Class, AttributeTargetTypes = "Metalama.Extensions.Multicast.AspectTests.Filter_ByTypeName.C`1" )]
[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "C<T>+N", AttributeTargetElements = MulticastTargets.Class, AttributeTargetTypes = "Metalama.Extensions.Multicast.AspectTests.Filter_ByTypeName.C`1+N" )]
[Tag( "C" )]
// <target>
public class C
{
    [Tag( "C+N" )]
    class N
    {
    }
}
public class D
{
}
[Tag( "C<T>" )]
// <target>
public class C<T>
{
    [Tag( "C<T>+N" )]
    class N
    {
    }
}