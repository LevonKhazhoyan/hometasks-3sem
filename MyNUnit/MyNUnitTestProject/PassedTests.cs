namespace MyNUnitTestProject;

using MyNUnitAttributes.Attributes;

public class PassingTests
{
    private static string myString;

    [BeforeClass]
    public static void BeforeTests()
        => myString = "NUnitTestString";

    [Test(Ignore = "No need")]
    public static void IgnoredTest()
        => myString.Concat("LL");

    [Test]
    public static void ContainsTest()
    {
        if (!myString.Contains('N'))
        {
            throw new Exception();
        }
    }

    [Test]
    public static void EndsWithTest()
    {
        if (!myString.EndsWith("ing"))
        {
            throw new Exception();
        }
    }

    [Test]
    public static void StringLengthTest()
    {
        if (myString.Length != 15)
        {
            throw new Exception();
        }
    }

    [AfterClass]
    public static void AfterTests()
    {
        if (myString != "NUnitTestString")
        {
            throw new Exception();
        }
    }
}