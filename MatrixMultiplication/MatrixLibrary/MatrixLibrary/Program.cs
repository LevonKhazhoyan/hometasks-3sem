using System.Diagnostics;
using MatrixLibrary.impl;

namespace MatrixLibrary;

static class Program
{
    /// <summary>
    /// Multiplying two given matrices, then write result to specified path.
    /// </summary>
    static void Main(string[] args)
    {
        Console.WriteLine("Insert -m to multiply 2 matrices or " +
                          "-p to show performance of multi-threaded matrix multiplication");

        switch (Console.ReadLine())
        {
            case "-m":
            {
                Console.WriteLine("Insert path to first matrix");
                var firstMatrix = Utils.Read(Console.ReadLine()!);
                Console.WriteLine("Insert path to second matrix");
                var secondMatrix = Utils.Read(Console.ReadLine()!);
                var result = firstMatrix.Multiply(secondMatrix, new SequentialMatrixMultiplication());
                Console.WriteLine("Insert path to place result matrix");
                Utils.Print(Console.ReadLine()!, result);
                break;
            }
            case "-p":
            {
                static void MatrixMultiplicationPerformance(int iterations, int size)
                {
                    var sequentialResults = new List<long>();
                    var parallelResults = new List<long>();
                    var timer = new Stopwatch();
                    var sequentialStrategy = new SequentialMatrixMultiplication();
                    var parallelStrategy = new ParallelMatrixMatrixMultiplication();
                    for (var i = 0; i < iterations; i++)
                    {
                        var fst = Utils.GenerateMatrix(size, size);
                        var snd = Utils.GenerateMatrix(size, size);

                        timer.Restart();
                        fst.Multiply(snd, sequentialStrategy);
                        timer.Stop();
                        sequentialResults.Add(timer.ElapsedMilliseconds);

                        timer.Restart();
                        fst.Multiply(snd, parallelStrategy);
                        timer.Stop();
                        parallelResults.Add(timer.ElapsedMilliseconds);
                    }

                    (double, double) FindAverageAndStandardDeviation(List<long> results)
                    {
                        var average = results.Average();
                        var variance = results.Select(x =>
                            Math.Pow(x - average, 2)).Average();
                        var standardDeviation = Math.Sqrt(variance);
                        return (average, standardDeviation);
                    }

                    var (averageSingleThreaded, standardDeviationSingleThreaded) =
                        FindAverageAndStandardDeviation(sequentialResults);
                    var (averageMultiThreaded, standardDeviationMultiThreaded) =
                        FindAverageAndStandardDeviation(parallelResults);
                    Console.WriteLine($"Size: {size}");
                    Console.WriteLine($"Average time for single-threaded: {averageSingleThreaded} ms");
                    Console.WriteLine($"Standard deviation for single-threaded: {standardDeviationSingleThreaded} ms");
                    Console.WriteLine($"Average time for multi-threaded: {averageMultiThreaded} ms");
                    Console.WriteLine($"Standard deviation for multi-threaded: {standardDeviationMultiThreaded} ms");
                }

                foreach (var pow in new [] { 7, 8, 9})
                {
                    MatrixMultiplicationPerformance(20, (int)Math.Pow(2, pow));
                }

                break;
            }
        }
    }
}