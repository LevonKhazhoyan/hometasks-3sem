using Lazy;
using NUnit.Framework;

namespace LazyUnitTests;

public class Tests
{
    [Test]
    public void SupplierCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateLazy<object>(null));
        Assert.Throws<ArgumentNullException>(() => LazyFactory.CreateConcurrentLazy<object>(null));
    }

    private static List<TestCaseData> Lazies()
    {
        var testCaseData = new List<TestCaseData>();

        foreach (var item in TestData)
        {
            testCaseData.Add(new TestCaseData(item, LazyFactory.CreateLazy(() => item)));
            testCaseData.Add(new TestCaseData(item, LazyFactory.CreateConcurrentLazy(() => item)));
        }

        return testCaseData;
    }

    [TestCaseSource(nameof(Lazies))]
    public void GetShouldNotChangeTheValueOnSingleCall<T>(object item, ILazy<T> lazy)
        => Assert.That(item, Is.EqualTo(lazy.Get()));
    
    [TestCaseSource(nameof(Lazies))]
    public void GetShouldNotChangeTheValueOnMultipleCalls(object item, ILazy<object> lazy)
    {
        var value = lazy.Get();
        Assert.That(value, Is.EqualTo(item));

        for (var i = 0; i < 100; i++)
        {
            Assert.That(value, Is.EqualTo(lazy.Get()));
        }
    }

    [Test]
    public void RacesCheckAndConcurrentLazeDoEvaluationOnce()
    {
        var counter = 0;
        var threads = new Thread[1000];
        var lazy = LazyFactory.CreateConcurrentLazy(() => Interlocked.Increment(ref counter));

        for (var i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() => {
                Assert.That(lazy.Get(), Is.EqualTo(1));
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    [Test]
    public void LazyDoEvaluationOnce()
    {
        var counter = 0;
        var lazy = LazyFactory.CreateLazy(() => Interlocked.Increment(ref counter));

        for (var i = 0; i < 100; i++)
        {
            Assert.That(lazy.Get(), Is.EqualTo(1));
            
        }
    }
    
    private static readonly object[] TestData =
    {
        1,
        10,
        100,
        1000
    };
}
