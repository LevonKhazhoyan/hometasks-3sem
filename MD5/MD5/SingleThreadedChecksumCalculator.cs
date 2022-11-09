namespace MD5;

using System.Collections.Concurrent;

public class SingleThreadedChecksumCalculator : ChecksumCalculator
{
    /// <summary>
    /// Calculates directory hashes in single thread
    /// </summary>
    public override ConcurrentDictionary<string, byte[]> CalculateHashes(IEnumerable<string> items)
    {
        var dictionary = new ConcurrentDictionary<string, byte[]>();
        foreach (var item in items)
        {
            var hash = File.Exists(item) ? CalculateFileHash(item) : CalculateDirectoryHash(item);
            dictionary[item] = hash;
        }
        return dictionary;
    }
}