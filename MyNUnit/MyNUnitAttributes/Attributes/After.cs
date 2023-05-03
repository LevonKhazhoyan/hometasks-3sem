namespace MyNUnitAttributes.Attributes;

/// <summary>
/// Indicates that method should run after tests
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class After : Attribute
{
    
}