namespace Lazy;

/// <summary>
/// Lazy evaluation interface.
/// </summary>
/// <typeparam name="T">Type of returning object.</typeparam>
public interface ILazy<out T>
{
    /// <summary>
    /// Calculate and return the value or return previously calculated one.
    /// </summary>
    /// <returns>Computation result.</returns>
    T Get();
}