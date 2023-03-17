namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// ThreadPool implementation 
/// </summary>
public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly ConcurrentQueue<Action> tasksQueue;
    private readonly ManualResetEvent queueBlocker;
    private readonly CancellationTokenSource cancellationTokenFactory;
    private volatile int amountOfSleepingThreads;
    private volatile bool isTimeToRetire;

    public MyThreadPool(int sizeOfPool)
    {
        threads = new Thread[sizeOfPool];
        tasksQueue = new ConcurrentQueue<Action>();
        queueBlocker = new ManualResetEvent(false);
        cancellationTokenFactory = new CancellationTokenSource();
        amountOfSleepingThreads = 0;
        for (var i = 0; i < sizeOfPool; i++)
        {
            threads[i] = new Thread(ThreadProcessing)
            {
                IsBackground = true
            };

            threads[i].Start();
        }
    }

    /// <summary>
    /// Add task to thread pool
    /// </summary>
    public IMyTask<T> Submit<T>(Func<T> function)
    {
        lock (cancellationTokenFactory)
        {
            if (cancellationTokenFactory.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }

            var myTask = new MyTask<T>(function, this);
            tasksQueue.Enqueue(myTask.Start);
            queueBlocker.Set();
            return myTask;
        }
    }

    /// <summary>
    /// Thread processing of tasksQueue
    /// </summary>
    private void ThreadProcessing()
    {
        while (!cancellationTokenFactory.IsCancellationRequested)
        {
            Action? task;
            while (!tasksQueue.TryDequeue(out task) && !cancellationTokenFactory.IsCancellationRequested)
            {
                queueBlocker.Reset();
                queueBlocker.WaitOne();
            }

            task?.Invoke();
        }

        while (!isTimeToRetire)
        {
            Action? task;
            while (!tasksQueue.TryDequeue(out task) && !isTimeToRetire)
            {
                queueBlocker.Reset();
                if (amountOfSleepingThreads == threads.Length - 1)
                {
                    isTimeToRetire = true;
                    queueBlocker.Set();
                    break;
                }

                Interlocked.Increment(ref amountOfSleepingThreads);
                queueBlocker.WaitOne();
                Interlocked.Decrement(ref amountOfSleepingThreads);
            }

            task?.Invoke();
        }
    }

    /// <summary>
    /// Waits for all currently working threads and doesn't allow new tasks to start execution
    /// </summary>
    public void Shutdown()
    {
        lock (cancellationTokenFactory)
        {
            cancellationTokenFactory.Cancel();
            queueBlocker.Set();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly ConcurrentQueue<Action> continuationQueue;
        private readonly MyThreadPool threadPool;
        private AggregateException? exception;
        private readonly ManualResetEvent getResultBlocker;
        private Func<TResult>? func;
        private TResult? result;

        /// <inheritdoc/>
        public bool IsCompleted => result != null;

        public MyTask(Func<TResult> function, MyThreadPool attached)
        {
            threadPool = attached;
            func = function;
            getResultBlocker = new ManualResetEvent(false);
            continuationQueue = new ConcurrentQueue<Action>();
        }
        
        /// <inheritdoc/>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> continuation)
        {
            lock (threadPool.cancellationTokenFactory)
            {
                if (threadPool.cancellationTokenFactory.IsCancellationRequested)
                {
                    throw new InvalidOperationException();
                }

                if (IsCompleted)
                {
                    return threadPool.Submit(() => continuation(result));
                }

                var taskOfContinuation = new MyTask<TNewResult>(() => continuation(Result), threadPool);
                continuationQueue.Enqueue(taskOfContinuation.Start);
                return taskOfContinuation;
            }
        }

        /// <inheritdoc/>
        public TResult? Result
        {
            get
            {
                getResultBlocker.WaitOne();
                if (exception != null)
                {
                    throw exception;
                }

                return result;
            }
        }
        
        /// <summary>
        /// Starts task
        /// </summary>
        public void Start()
        {
            try
            {
                result = func();
                func = null;
            }
            catch (Exception e)
            {
                exception = new AggregateException(e);
            }

            getResultBlocker.Set();

            foreach (var continuation in continuationQueue)
            {
                threadPool.tasksQueue.Enqueue(continuation);
                threadPool.queueBlocker.Set();
            }
        }
    }
}