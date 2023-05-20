global using System;
global using System.Collections.Generic;
global using System.Linq;
using System.Diagnostics;
using MD5;

if (args.Length != 1)
{
    Console.WriteLine("Incorrect number of arguments. Please, specify only path to directory or file");
    return;
}

try
{
    var checksumCalculator = new MultiThreadedChecksumCalculator();
    var hash = checksumCalculator.Calculate(args[0]);
    Console.WriteLine(BitConverter.ToString(hash));
}
catch (ArgumentException error)
{
    Console.WriteLine(error.Message);
}

var resultsSingleThreaded = new List<long>();
var resultsMultiThreaded = new List<long>();
var timer = new Stopwatch();
var singleThreadedChecksumCalculator = new SingleThreadedChecksumCalculator();
var multiThreadedChecksumCalculator = new MultiThreadedChecksumCalculator();
for (var i = 0; i < 10; i++)
{
    timer.Restart();
    singleThreadedChecksumCalculator.Calculate(args[0]);
    timer.Stop();
    resultsSingleThreaded.Add(timer.ElapsedMilliseconds);

    timer.Restart();
    multiThreadedChecksumCalculator.Calculate(args[0]);
    timer.Stop();
    resultsMultiThreaded.Add(timer.ElapsedMilliseconds);
}

var singleThreadedAverage = resultsSingleThreaded.Average();
var singleThreadedDispersion = resultsSingleThreaded.Select(x => Math.Pow(x - singleThreadedAverage, 2)).Average();
var singleThreadedStandardDeviation = Math.Sqrt(singleThreadedDispersion);
var multiThreadedAverage = resultsMultiThreaded.Average();
var multiThreadedDispersion = resultsMultiThreaded.Select(x => Math.Pow(x - singleThreadedAverage, 2)).Average();
var multiThreadedStandardDeviation = Math.Sqrt(multiThreadedDispersion);
Console.WriteLine($"Single threaded average and deviation: {singleThreadedAverage}, {singleThreadedStandardDeviation}");
Console.WriteLine($"Multi threaded average and deviation: {multiThreadedAverage}, {multiThreadedStandardDeviation}");