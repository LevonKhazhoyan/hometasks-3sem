using NetworkChat;

if (args.Length is 0 or > 2)
{
    Console.WriteLine("Input one argument to listen port as a server. Input two arguments to connect to server as a client.");
    return;
}

var isValidPort = int.TryParse(args[0], out var port);
if (!isValidPort)
{
    Console.WriteLine("Expected valid port.");
    return;
}

if (args.Length == 1)
{
    var server = new Server(port);
    await server.Start();
}
else
{
    var client = new Client(port, args[1]);
    await client.Start();
}