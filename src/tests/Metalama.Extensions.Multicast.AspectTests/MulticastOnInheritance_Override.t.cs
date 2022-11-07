[AddTagInherited( "Tagged", true )]
[Tag( "Tagged" )]
public class C
{
    [Tag( "Tagged" )]
    protected virtual void M()
    {
    }
}
[Tag( "Tagged" )]
// <target>
public class D : C
{
    [AddTagInherited( "Overridden" )]
    [Tag( "Overridden" )]
    protected override void M()
    {
    }
    [Tag( "Tagged" )]
    protected void N()
    {
    }
}
