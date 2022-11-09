using System.Collections.Concurrent;
using System.Text;

namespace MD5;

using System.Security.Cryptography;

/// <summary>
/// Implementation of checksum calculator
/// </summary>
public abstract class ChecksumCalculator
{
    
    /// <summary>
    /// Calculates checksum of directory or file
    /// </summary>
    /// <param name="path">Path to directory of file where checksum must be calculated</param>
    /// <returns>Checksum in byte[] format</returns>
    /// <exception cref="ArgumentException">Specified path doesn't exist</exception>
    public byte[] Calculate(string path)
    {
        if (File.Exists(path))
        {
            return CalculateFileHash(path);
        }

        if (!Directory.Exists(path)) throw new ArgumentException("Invalid path");

        return CalculateDirectoryHash(path);
    }

    /// <summary>
    /// Calculates checksum of file
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns>Checksum in byte[] format</returns>
    protected static byte[] CalculateFileHash(string path)
    {
        using var file = File.OpenRead(path);
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(file);
        return hash;
    }

    /// <summary>
    /// Gets all subdirectories and files in directory
    /// </summary>
    /// <param name="path">Path to directory</param>
    /// <returns>IEnumerable of items</returns>
    protected static IEnumerable<string> GetAllDirectoryItems(string path)
    {
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        var items = files.Concat(directories);
        return items;
    }

    
    /// <summary>
    /// Calculates hashes for directory
    /// </summary>
    public abstract ConcurrentDictionary<string, byte[]> CalculateHashes(IEnumerable<string> items);

    /// <summary>
    /// Calculates directory checksum
    /// </summary>
    /// <param name="path">Path to directory</param>
    /// <returns>Checksum in byte[] format</returns>
    public byte[] CalculateDirectoryHash(string path)
    {
        var sorted = CalculateHashes(GetAllDirectoryItems(path)).OrderBy(x => x.Key);
        var bytes = Encoding.UTF8.GetBytes(Path.GetDirectoryName(path)!);
        foreach (var (_, value) in sorted)
        {
            bytes = bytes.Concat(value).ToArray();
        }

        using var md5 = MD5.Create();
        var result = md5.ComputeHash(bytes);
        return result;
    }
}