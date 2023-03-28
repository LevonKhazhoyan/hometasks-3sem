namespace MyNUnitAttributes.Attributes;

/// <summary>
/// Test method attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Test : Attribute
{
    // Expected exception type
    public Type? Expected { get; set; }

    // Ignore reason
    public string? Ignore { get; set; }
}