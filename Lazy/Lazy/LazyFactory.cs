namespace Lazy;

/// <summary>
/// Factory of ILazy instance.
/// </summary>
public static class LazyFactory
{
    /// <summary>
    /// Creates an instance of <see cref="Lazy{T}"/>
    /// </summary>
    /// <param name="supplier"> Function to evaluate </param>
    /// <typeparam name="T"> Returning type of function</typeparam>
    /// <returns>Instance of <see cref="Lazy{T}"/></returns>
    public static ILazy<T> CreateLazy<T>(Func<T> supplier)
        => new Lazy<T>(supplier);

    /// <summary>
    /// Creates an instance of <see cref="ConcurrentLazy{T}"/>
    /// </summary>
    /// <param name="supplier"> Function to evaluate </param>
    /// <typeparam name="T"> Returning type of function</typeparam>
    /// <returns>Instance of <see cref="ConcurrentLazy{T}"/></returns>
    public static ILazy<T> CreateConcurrentLazy<T>(Func<T> supplier) 
        => new ConcurrentLazy<T>(supplier);
}