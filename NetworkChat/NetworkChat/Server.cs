using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

public class Server
{
    private readonly TcpListener listener;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }

    /// <summary>
    /// Starts server
    /// </summary>
    public async Task Start()
    {
        listener.Start();
        var client = await listener.AcceptTcpClientAsync();

        await using (stream = client.GetStream())
        {
            Receive();
            await Send();
        }
    }

    /// <summary>
    /// Receives messages
    /// </summary>
    private void Receive()
    {
        Task.Run(async () =>
        {
            using (reader = new StreamReader(stream))
            {
                var received = await reader.ReadLineAsync();
                while (received != "exit")
                {
                    Console.WriteLine(received);
                    received = await reader.ReadLineAsync();
                }

                listener.Stop();
                Environment.Exit(0);
            }
        });
    }

    /// <summary>
    /// Sends messages
    /// </summary>
    private Task Send()
    {
        return Task.Run(async () =>
        {
            await using (writer = new StreamWriter(stream) { AutoFlush = true })
            {
                var tasks = new List<Task>();
                var message = Console.ReadLine();
                while (message != "exit")
                {
                    tasks.Add(writer.WriteLineAsync(message));
                    message = Console.ReadLine();
                }

                Task.WaitAll(tasks.ToArray());
                listener.Stop();
                Environment.Exit(0);
            }
        });
    }
}