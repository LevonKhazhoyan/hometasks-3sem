namespace MyThreadPoolTests;

using System.Diagnostics;
using NUnit.Framework;
using MyThreadPool;

public class Tests
{
    private const int NumberOfThreads = 10;
    private MyThreadPool myTestThreadPool = new(NumberOfThreads);

    [SetUp]
    public void Setup()
        => myTestThreadPool = new MyThreadPool(NumberOfThreads);
    

    [OneTimeTearDown]
    public void Teardown()
        => myTestThreadPool.Shutdown();
    
    [Test]
    public void MinimumRunningThreadsCount()
        => Assert.That(Process.GetCurrentProcess().Threads, Has.Count.GreaterThan(NumberOfThreads));
    
    [Test]
    public void ContinueWithWorks()
    {
        var task = myTestThreadPool.Submit(() => 1);
        var continuation = task.ContinueWith(value => value + 1);
        Assert.That(continuation.Result, Is.EqualTo(2));
    }

    [Test]
    public void ContinueWithWorksAfterShutdown()
    {
        var task = myTestThreadPool.Submit(() =>
        {
            Thread.Sleep(500);
            return false;
        });
        var continuation = task.ContinueWith(value => !value);
        myTestThreadPool.Shutdown();
        Assert.That(continuation.Result, Is.EqualTo(true));
    }

    [Test]
    public void MultiThreadingTest()
    {
        var threads = new Task[NumberOfThreads];
        var count = 0;
        for (var i = 0; i < NumberOfThreads - 2; i++)
        {
            threads[i] = Task.Run(() =>
            {
                var task = myTestThreadPool.Submit(() => 0);
                task.ContinueWith(v =>
                {
                    Thread.Sleep(1000);
                    Interlocked.Increment(ref count);
                    return 0;
                });
            });
        }

        threads[NumberOfThreads - 2] = Task.Run(() =>
        {
            var task = myTestThreadPool.Submit(() =>
            {
                Thread.Sleep(200);
                Interlocked.Increment(ref count);
                return 0;
            });

            task.ContinueWith(v =>
            {
                Thread.Sleep(1000);
                Interlocked.Increment(ref count);
                return 0;
            });
        });

        threads[NumberOfThreads - 1] = Task.Run(() =>
        {
            Thread.Sleep(500);
            myTestThreadPool.Shutdown();
        });

        for (var i = 0; i < NumberOfThreads; i++)
        {
            threads[i].Wait();
        }

        Assert.That(count, Is.EqualTo(NumberOfThreads));
    }
    
    [Test]
    [NonParallelizable]
    public void SubmittedTasksCompleteCalculationAfterShutdown()
    {
        var smallTp = new MyThreadPool(2);
        var tasks = new IMyTask<int>[2];
        for (var i = 0; i < 2; i++)
        {
            tasks[i] = smallTp.Submit(() =>
            {
                Thread.Sleep(500);
                return 1;
            });
        }

        var task = smallTp.Submit(() => 1);
        smallTp.Shutdown();
        Assert.That(task.Result, Is.EqualTo(1));
    }

    [Test]
    public void SubmitLeadsToExceptionAfterShutdown()
    {
        myTestThreadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => myTestThreadPool.Submit(() => 1));
    }
    
    [Test]
    public void ActuallyNThreads()
    {
        var count = 0;
        for (var i = 0; i < 100; i++)
        {
            myTestThreadPool.Submit(() =>
            {
                Thread.Sleep(100);
                Interlocked.Increment(ref count);
                return 1;
            });
        }

        Thread.Sleep(150);
        Assert.That(count, Is.EqualTo(NumberOfThreads));
    }
}