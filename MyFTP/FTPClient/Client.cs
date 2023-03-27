namespace MyFTPClient;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// FTP client class
/// </summary>
public class Client
{
    private readonly int port;
    private readonly IPAddress ip;

    /// <summary>
    /// Client's constructor
    /// </summary>
    public Client(IPAddress ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }

    /// <summary>
    /// Client's constructor
    /// </summary>
    public Client(string ip, int port)
    {
        this.ip = IPAddress.Parse(ip);
        this.port = port;
    }

    /// <summary>
    /// Shows list of files in the directory and directories by itself
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <param name="token">Cancellation token for holding connection</param>
    public async Task<List<(string name, bool isDir)>> ListAsync(string path, CancellationToken token)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip.ToString(), port, token);
        await using var stream = client.GetStream();
        await using var writer = new StreamWriter(stream){AutoFlush = true};
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync($"1 {path}");
        
        var response = await reader.ReadLineAsync();
        var files = new List<(string, bool)>();
        
        if (response is null)
        {
            return files;
        }
        
        var partitionedResponse = response.Split(' ');
        var countOfFiles = int.Parse(partitionedResponse[0]);
        
        if (countOfFiles == -1)
        {
            throw new DirectoryNotFoundException();;
        }
        
        for (var i = 1; i <= 2 * countOfFiles; i += 2)
        {
            var name = partitionedResponse[i];
            var isDir = bool.Parse(partitionedResponse[i + 1]);
            files.Add((name, isDir));
        }
        
        return files;
    }

    /// <summary>
    /// Downloads file and its size
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="responseStream">Stream to write the response data to</param>
    /// <param name="token">Cancellation token for holding connection</param>
    public async Task GetAsync(string path, Stream responseStream, CancellationToken token)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip.ToString(), port, token);
        await using var stream = client.GetStream();
        await using var writer = new StreamWriter(stream) {AutoFlush = true};
        await writer.WriteLineAsync($"2 {path}");

        using var reader = new StreamReader(stream);
        var parseResult = long.TryParse(await reader.ReadLineAsync(), out var size);
        if (size == -1 || !parseResult)
        {
            throw new FileNotFoundException();
        }

        await stream.CopyToAsync(responseStream, token);
        responseStream.Position = 0;
    }
}