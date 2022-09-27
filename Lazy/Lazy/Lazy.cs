namespace Lazy;

/// <summary>
/// Evaluate supplier once and return previously calculated result.
/// </summary>
/// <typeparam name="T"> Type of supplier result. </typeparam>
public class Lazy<T> : ILazy<T>
{
    private Func<T> supplier;
    private bool isComputed;
    private T result;

    /// <summary>
    /// Create an instance of <see cref="Lazy{T}"/>.
    /// </summary>
    /// <param name="supplier"> Function to evaluate. </param>
    /// <exception cref="ArgumentNullException"> Throws exception, when supplier is null. </exception>
    public Lazy(Func<T> supplier)
        => this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));

    /// <inheritdoc/>
    public T Get()
    {
        if (isComputed)
        {
            return result;
        }

        result = supplier();
        isComputed = true;
        supplier = null;
        return result;
    }
}