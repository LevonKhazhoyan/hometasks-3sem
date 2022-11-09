namespace MD5;

using System.Collections.Concurrent;

public class MultiThreadedChecksumCalculator : ChecksumCalculator
{
    /// <summary>
    /// Calculates directory hashes in parallel
    /// </summary>
    public override ConcurrentDictionary<string, byte[]> CalculateHashes(IEnumerable<string> items)
    {
        var dictionary = new ConcurrentDictionary<string, byte[]>();
        Parallel.ForEach(items, item =>
        {
            var hash = File.Exists(item) ? CalculateFileHash(item) : CalculateDirectoryHash(item);
            dictionary[item] = hash;
        });
        return dictionary;
    }
}