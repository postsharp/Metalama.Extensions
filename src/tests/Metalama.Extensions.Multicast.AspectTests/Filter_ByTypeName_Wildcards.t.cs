[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "Prefixed", AttributeTargetElements = MulticastTargets.Class, AttributeTargetTypes = "Metalama.Extensions.Multicast.AspectTests.Filter_ByTypeName_Wildcards.Prefixed*" )]
[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "Ns", AttributeTargetElements = MulticastTargets.Class, AttributeTargetTypes = "Metalama.Extensions.Multicast.AspectTests.Filter_ByTypeName_Wildcards.Ns.*" )]
[Tag( "Prefixed" )]
// <target>
public class PrefixedA
{
}
[Tag( "Prefixed" )]
// <target>
public class PrefixedB
{
}
public class NonPrefixed
{
}
namespace Ns
{
    [Tag( "Ns" )]
    public class InNamespace
    {
    }
}