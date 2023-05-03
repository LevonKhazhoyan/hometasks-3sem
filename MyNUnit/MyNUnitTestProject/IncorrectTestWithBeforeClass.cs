namespace MyNUnitTestProject;

using MyNUnitAttributes.Attributes;

public class IncorrectTestWithBeforeClass
{
    private int a = 1;
    
    [BeforeClass]
    public static void MethodThrowException()
    {
        throw new AggregateException();
    }
    
    [Test]
    public void TestFromBeforeTestFail()
        => a++;
}