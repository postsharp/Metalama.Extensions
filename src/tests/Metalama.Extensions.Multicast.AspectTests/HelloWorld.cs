using Metalama.Framework.Aspects;

[assembly: Metalama.Extensions.Multicast.AspectTests.HelloWorld.MyAspect( "Hello, world." )]

namespace Metalama.Extensions.Multicast.AspectTests.HelloWorld
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