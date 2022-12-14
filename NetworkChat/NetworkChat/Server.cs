using System.Net;
using System.Net.Sockets;

namespace NetworkChat;

public class Server
{
    private readonly TcpListener _listener;
    private NetworkStream _stream;
    private StreamWriter _writer;
    private StreamReader _reader;

    public Server(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    /// <summary>
    /// Starts server
    /// </summary>
    public async Task Start()
    {
        _listener.Start();
        var client = await _listener.AcceptTcpClientAsync();

        await using (_stream = client.GetStream())
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
            using (_reader = new StreamReader(_stream))
            {
                var received = await _reader.ReadLineAsync();
                while (received != "exit")
                {
                    Console.WriteLine(received);
                    received = await _reader.ReadLineAsync();
                }

                _listener.Stop();
                Environment.Exit(0);
            }
        });
    }

    /// <summary>
    /// Sends messages
    /// </summary>
    /// <returns>Task</returns>
    private Task Send()
    {
        return Task.Run(async () =>
        {
            await using (_writer = new StreamWriter(_stream) { AutoFlush = true })
            {
                var tasks = new List<Task>();
                var message = Console.ReadLine();
                while (message != "exit")
                {
                    tasks.Add(_writer.WriteLineAsync(message));
                    message = Console.ReadLine();
                }

                Task.WaitAll(tasks.ToArray());
                _listener.Stop();
                Environment.Exit(0);
            }
        });
    }
}