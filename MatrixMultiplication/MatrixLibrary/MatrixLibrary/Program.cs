using System.Diagnostics;
using MatrixLibrary;
using MatrixLibrary.Impl;

switch (args[0])
{
    case "-m":
    {
        try
        {
            var firstMatrix = Utils.Read(args[1]);
            var secondMatrix = Utils.Read(args[2]);
            var result = firstMatrix.Multiply(secondMatrix, new SequentialMatrixMultiplication());
            Utils.Print(args[3], result);
        }
        catch (Exception ex) when (
            ex is IndexOutOfRangeException
            or ArgumentException)
        {
            Console.WriteLine("\nExpected:" +
                  "\nFirst argument should be path to first matrix to multiply" +
                  "\nSecond argument should be path to second matrix to multiply" +
                  "\nThird argument should be path to multiply result matrix");
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
        


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

        foreach (var pow in new [] { 7, 8, 9 })
        {
            MatrixMultiplicationPerformance(20, (int)Math.Pow(2, pow));
        }

        break;
    }
    default:
    {
        Console.WriteLine("Expected \"-m\" to multiply 2 matrices or \"-p\" to show performance of multiplication as first argument");
        break;
    }
}
