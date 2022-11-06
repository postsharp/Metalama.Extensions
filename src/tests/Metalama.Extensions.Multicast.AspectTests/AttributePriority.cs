using Metalama.Framework.Aspects;

[assembly: Metalama.Extensions.Multicast.AspectTests.AttributePriority.MyAspect( "1", AttributePriority = 1 )]
[assembly: Metalama.Extensions.Multicast.AspectTests.AttributePriority.MyAspect( "2", AttributePriority = 2 )]

namespace Metalama.Extensions.Multicast.AspectTests.AttributePriority
{
    public class MyAspect : OverrideMethodMulticastAspect
    {
        private readonly string _tag;

        public MyAspect( string tag )
        {
            this._tag = tag;
        }

        public override dynamic? OverrideMethod()
        {
            Console.WriteLine($"Overridden: {this._tag}");

            return meta.Proceed();
        }
    }


    // <target>
    public class C
    {
        public void M()
        {
            
        }
    }

}