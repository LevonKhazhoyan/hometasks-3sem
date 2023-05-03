namespace MyNUnit.MyNUnitImpl;

using System.Reflection;

/// <summary>
/// Store class methods marked with custom attributes
/// </summary>
public class MethodsList
{
    /// <summary>
    /// List of methods containing 'After' attribute
    /// </summary>
    public readonly List<MethodInfo> After = new();
    /// <summary>
    /// List of methods containing 'AfterClass' attribute
    /// </summary>
    public readonly List<MethodInfo> AfterClass  = new();
    /// <summary>
    /// List of methods containing 'Before' attribute
    /// </summary>
    public readonly List<MethodInfo> Before  = new();
    /// <summary>
    /// List of methods containing 'BeforeClass' attribute
    /// </summary>
    public readonly List<MethodInfo> BeforeClass = new();
    /// <summary>
    /// List of methods containing 'Test' attribute
    /// </summary>
    public readonly List<MethodInfo> Test = new();
}