using System.Net.Sockets;

namespace NetworkChat;

public class Client : IDisposable
{
    private TcpClient client;
    private NetworkStream stream;
    private readonly int port;
    private readonly string ip;
    private readonly CancellationTokenSource cancellationTokenSource;
    
    public Client(int port, string ip)
    {
        this.port = port;
        this.ip = ip;
        cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Starts client
    /// </summary>
    public async Task Start()
    {
        client = new TcpClient(ip, port);
        await client.Client.ConnectAsync(ip, port);
        await using (stream = client.GetStream())
        {
            Receive();
            await Send();
        }
    }
    
    /// <summary>
    /// Receives message
    /// </summary>
    private void Receive()
    {
        Task.Run(async () =>
        {
            using var reader = new StreamReader(stream);
            var received = await reader.ReadLineAsync();
            while (received != "exit")
            {
                Console.WriteLine(received);
                received = await reader.ReadLineAsync();
            }

            cancellationTokenSource.Cancel();
            client.Close();
        });
    }
    
    /// <summary>
    /// Sends message
    /// </summary>
    private Task Send()
    {
        return Task.Run(async () =>
        {
            await using var writer = new StreamWriter(stream) {AutoFlush = true};
            var tasks = new List<Task>();
            var message = Console.ReadLine();
            while (message != "exit")
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
                tasks.Add(writer.WriteLineAsync(message));
                message = Console.ReadLine();
            }

            Task.WaitAll(tasks.ToArray());
            client.Close();
        });
    }
    
    public void Dispose()
    {
        cancellationTokenSource.Dispose();
        client.Dispose();
    }
}