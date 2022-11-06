using Metalama.Framework.Aspects;

[assembly: Metalama.Extensions.Multicast.AspectTests.Exclude_IntermediateNode.MyAspect( "1" )]

namespace Metalama.Extensions.Multicast.AspectTests.Exclude_IntermediateNode
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true )]
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
    [MyAspect( "2", AttributeExclude = true )]
    public class C
    {
        public void M()
        {
            
        }
    }

}