namespace MyNUnitTests;

using MyNUnit.MyNUnitImpl;
using NUnit.Framework;

public class Tests
{
    private static bool Check(IEnumerable<TestResult> result, IEnumerable<(string, ResultState)> list)
    {
        var resultSet = new HashSet<(string, ResultState)>(list);
        var testResultSet = new HashSet<(string, ResultState)>(result.Select(r => (r.Name, r.Result)));
        return resultSet.SetEquals(testResultSet);
    }

    [TestCase("../../../../MyNUnitTestProject/bin")]
    public void Test(string path)
    {
        var result = new List<(string, ResultState)>
        {
            ("NoExceptionTest", ResultState.Failed),
            ("TestFromIgnoredTest", ResultState.Ignored),
            ("IgnoredTest", ResultState.Ignored),
            ("ContainsTest", ResultState.Passed),
            ("EndsWithTest", ResultState.Passed),
            ("StringLengthTest", ResultState.Passed),
            ("NotMatchingExceptionsTest", ResultState.Failed),
            ("MatchingExceptionsTest", ResultState.Passed),
            ("TestFromFailingBeforeClass", ResultState.Ignored),
            ("TestFromBeforeTestFail", ResultState.Ignored),
            ("ExceptionTest", ResultState.Failed)
        };
        var myNUnitClass = new MyNUnitClass();
        myNUnitClass.Start(path);
        Assert.That(Check(myNUnitClass.ResultList, result), Is.True);
    }
}