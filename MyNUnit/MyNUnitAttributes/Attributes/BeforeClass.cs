namespace MyNUnitAttributes.Attributes;

/// <summary>
/// Indicates that method should run before tests
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClass : Attribute
{

}