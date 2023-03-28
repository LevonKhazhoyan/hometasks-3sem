namespace MyNUnitTestProject;

using MyNUnitAttributes.Attributes;

public class ExceptionsTests
{
    private int a = 1;

    [Test(Expected = typeof(ArgumentNullException))]
    public void NoExceptionTest()
        => a++;

    [Test(Expected = typeof(ArgumentNullException))]
    public static void NotMatchingExceptionsTest()
        => throw new ArgumentOutOfRangeException();

    [Test(Expected = typeof(ArgumentNullException))]
    public static void MatchingExceptionsTest()
        => throw new ArgumentNullException();

    [Test]
    public static void ExceptionTest()
        => throw new ArgumentException();
}