namespace Lazy;

/// <summary>
/// Evaluate supplier once and return previously calculated result. Thread-safe.
/// </summary>
/// <typeparam name="T"> Type of supplier result. </typeparam>
public class ConcurrentLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private readonly object myLock;
    private T? result;
    private volatile bool isComputed;

    /// <summary>
    /// Create an instance of <see cref="ConcurrentLazy{T}"/>.
    /// </summary>
    /// <param name="supplier"> Function to evaluate. </param>
    /// <exception cref="ArgumentNullException"> Throws exception, when supplier is null. </exception>
    public ConcurrentLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        myLock = new object();
        isComputed = false;
    }

    /// <inheritdoc/>
    public T? Get()
    {
        if (isComputed) return result;
        lock (myLock)
        {
            if (isComputed) return result;
            result = supplier!();
            isComputed = true;
            supplier = null;
        }

        return result;
    }
}