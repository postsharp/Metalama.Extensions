[assembly: Metalama.Extensions.Multicast.AspectTests.AddTag( "Tagged" )]
namespace Metalama.Extensions.Multicast.AspectTests.AddToAllFromCompilation
{
    [Tag( "Tagged" )]
    public class AClass
    {
        [Tag( "Tagged" )]
        [return: Tag( "Tagged" )]
        public int Method( [Tag( "Tagged" )] int p ) => 0;
        [Tag( "Tagged" )]
        public int Property
        {
            [Tag( "Tagged" )]
            [return: Tag( "Tagged" )]
            get; [Tag( "Tagged" )]
            private set;
        }
        [Tag( "Tagged" )]
        public int Field;
        [Tag( "Tagged" )]
        public event Action PublicEvent;
    }
    [Tag( "Tagged" )]
    public class AStruct
    {
        [Tag( "Tagged" )]
        class Nested
        {
            [Tag( "Tagged" )]
            [return: Tag( "Tagged" )]
            public int Method( [Tag( "Tagged" )] int p ) => 0;
            [Tag( "Tagged" )]
            public int Property
            {
                [Tag( "Tagged" )]
                [return: Tag( "Tagged" )]
                get; [Tag( "Tagged" )]
                private set;
            }
            [Tag( "Tagged" )]
            public int Field;
            [Tag( "Tagged" )]
            public event Action PublicEvent;
        }
    }
    // TODO: currently adding an attribute to an enum or delegate does nothing.
    public enum AnEnum
    {
    }
    public delegate void ADelegate();
}
