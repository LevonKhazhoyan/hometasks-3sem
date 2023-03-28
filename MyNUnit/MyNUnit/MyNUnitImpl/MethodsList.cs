namespace MyNUnit.MyNUnitImpl;

using System.Reflection;

/// <summary>
/// Store class methods marked with custom attributes
/// </summary>
public class MethodsList
{
    public List<MethodInfo> After { get; } = new();
    public List<MethodInfo> AfterClass { get; } = new();
    public List<MethodInfo> Before { get; } = new();
    public List<MethodInfo> BeforeClass { get; } = new();
    public List<MethodInfo> Test { get; } = new();
}