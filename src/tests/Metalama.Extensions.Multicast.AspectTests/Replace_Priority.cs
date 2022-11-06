using Metalama.Framework.Aspects;

[assembly: Metalama.Extensions.Multicast.AspectTests.Replace_Priority.MyAspect( "1", AttributePriority = 1 )]
[assembly: Metalama.Extensions.Multicast.AspectTests.Replace_Priority.MyAspect( "2", AttributePriority = 2 )]

namespace Metalama.Extensions.Multicast.AspectTests.Replace_Priority
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true )]
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