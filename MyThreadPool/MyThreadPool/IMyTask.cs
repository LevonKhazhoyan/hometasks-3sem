namespace MyThreadPool;

/// <summary>
/// Interface for MyTask, object that returns by MyThreadPool
/// </summary>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Checks if task is completed
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Returns the result of the task
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Adds a new task, the result of which depends on another task
    /// </summary>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func);
}