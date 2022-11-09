namespace ChecksumCalculatorTests;

using MD5;
using NUnit.Framework;

public class Tests
{
    private readonly string _path = Path.Join("..", "..", "..", "..", "ChecksumCalculatorTests", "TestData");
    private readonly SingleThreadedChecksumCalculator singleThreadedChecksumCalculator = new SingleThreadedChecksumCalculator();
    private readonly MultiThreadedChecksumCalculator multiThreadedChecksumCalculator = new MultiThreadedChecksumCalculator();

    [Test]
    public void SingleThreadedAndMultiThreadedShouldBeEquivalent()
    {
        for (var i = 0; i < 100; i++)
        {
            
            var hashSingleThreaded = singleThreadedChecksumCalculator.Calculate(_path);
            var hashMultiThreaded = multiThreadedChecksumCalculator.Calculate(_path);
            Assert.That(BitConverter.ToString(hashSingleThreaded), Is.EqualTo(BitConverter.ToString(hashMultiThreaded)));
        }
    }

    [Test]
    public void ChecksumShouldNotChange()
    {
        for (var i = 0; i < 100; i++)
        {
            var hash = multiThreadedChecksumCalculator.Calculate(_path);
            var secondHash = multiThreadedChecksumCalculator.Calculate(_path);
            Assert.That(BitConverter.ToString(hash), Is.EqualTo(BitConverter.ToString(secondHash)));
        }
    }
}