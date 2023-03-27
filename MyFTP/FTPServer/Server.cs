namespace MyFTPServer;

using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain;

public class Server
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly List<Task> tasks = new();
    private readonly TcpListener tcpListener;
    private readonly AutoResetEvent remainingTasksBlocker = new(false);

    public Server(IPAddress ip, int port)
        => tcpListener = new TcpListener(ip, port);

    public Server(string ip, int port)
        => tcpListener = new TcpListener(IPAddress.Parse(ip), port);

    /// <summary>
    /// Query processing method
    /// </summary>
    /// <param name="client">TCP client</param>
    /// <param name="token">Cancellation token for downloading file</param>
    private async Task ExecuteAsync(TcpClient client, CancellationToken token)
    {
        using (client)
        {
            await using var stream = client.GetStream();
            await using var writer = new StreamWriter(stream) {AutoFlush = true};
            using var reader = new StreamReader(stream);
            var requestString = await reader.ReadLineAsync();
            try
            {
                var request = ParseRequest(requestString!);
                switch (request.Type)
                {
                    case RequestType.List:
                        await ListAsync(request.Value, writer);
                        break;
                    case RequestType.Get:
                        await GetAsync(request.Value, writer, token);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Request.Type), "Invalid request type");
                }

                remainingTasksBlocker.Set();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException(nameof(requestString), "Input string cannot be null");
            }
        }
    }
 
    /// <summary>
    /// Starts server
    /// </summary>
    public async Task StartAsync()
    {
        tcpListener.Start();
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Token.Register(() => tcpListener.Stop());
            try
            {
                var client = await tcpListener.AcceptTcpClientAsync(cancellationTokenSource.Token);
                var clientTask = Task.Run(() => ExecuteAsync(client, cancellationTokenSource.Token));
                tasks.Add(clientTask);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Server disconnected");
                return;
            }
        }

        Task.WaitAll(tasks.ToArray());
        tcpListener.Stop();
    }
    
    private static Request ParseRequest(string input)
    {
        var parts = input.Split(' ');
        if (parts.Length != 2 || !int.TryParse(parts[0], out var type))
        {
            throw new ArgumentException("Invalid request format");
        }
        return new Request(type, parts[1]);
    }

    private static async Task ListAsync(string path, TextWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }
        
        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        var response = new StringBuilder();
        response.Append(files.Length + directories.Length);
        files.ToList().ForEach(x => response.Append($" {x} False"));
        directories.ToList().ForEach(x => response.Append($" {x} True"));
        await writer.WriteLineAsync(response.ToString());
    }

    private static async Task GetAsync(string path, StreamWriter writer, CancellationToken token)
    {
        var file = new FileInfo(path);
        if (!file.Exists)
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        await writer.WriteLineAsync(file.Length.ToString());
        await using var fileStream = file.OpenRead();
        await fileStream.CopyToAsync(writer.BaseStream, token);
    }

    public void Shutdown()
    {
        cancellationTokenSource.Cancel();
        Task.WaitAll();
        tcpListener.Stop();
    }
}