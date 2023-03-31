namespace MyNUnit.MyNUnitImpl;

/// <summary>
/// Class for tests result information
/// </summary>
public class TestResult
{
    /// <summary>
    /// Test name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Test result
    /// </summary>
    public ResultState Result { get; }
    /// <summary>
    /// Time test taken
    /// </summary>
    public TimeSpan ElapsedTime { get; }
    /// <summary>
    /// Reason of ignoring test
    /// </summary>
    public string? IgnoreReason { get; }

    public TestResult(string name, ResultState result, TimeSpan time, string? reason)
    {
        Name = name;
        Result = result;
        ElapsedTime = time;
        IgnoreReason = reason;
    }
}