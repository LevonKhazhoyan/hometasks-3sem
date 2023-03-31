namespace MyNUnit.MyNUnitImpl;

using System.Reflection;

/// <summary>
/// Store class methods marked with custom attributes
/// </summary>
public class MethodsList
{
    public readonly List<MethodInfo> After = new();
    public readonly List<MethodInfo> AfterClass  = new();
    public readonly List<MethodInfo> Before  = new();
    public readonly List<MethodInfo> BeforeClass = new();
    public readonly List<MethodInfo> Test = new();
}