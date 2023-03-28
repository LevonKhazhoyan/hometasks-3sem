namespace MyNUnitAttributes.Attributes;

/// <summary>
/// Indicates that method should run before each test
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Before : Attribute
{

}