namespace MyThreadPool;

/// <summary>
/// Interface for task in MyThreadPool
/// </summary>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Indicates whether the task has completed
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task, blocking until the task completes
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Creates a continuation that executes when the target task completes
    /// </summary>
    /// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
    /// <param name="continuation">A function to run when the task completes. Takes the result of the completed task as an argument.</param>
    /// <returns>A new task that represents the continuation.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}