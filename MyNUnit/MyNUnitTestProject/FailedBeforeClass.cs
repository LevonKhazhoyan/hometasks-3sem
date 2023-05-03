namespace MyNUnitTestProject;

using MyNUnitAttributes.Attributes;

public class FailedBeforeClass
{
    [BeforeClass]
    public static void BeforeTestsMethod()
        => throw new Exception();

    private int a = 1;

    [Test]
    public void TestFromFailingBeforeClass()
        => a++;
}