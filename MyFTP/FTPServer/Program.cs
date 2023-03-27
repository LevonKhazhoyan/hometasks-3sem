using MyFTPServer;

if (args.Length < 2)
{
    Console.WriteLine("Provide in args separated by spaces ip, port\n" +
                      "For example: 127.10.0.1 8888");
    return;
}

var server = new Server(args[0], int.Parse(args[1]));
var startServer = server.StartAsync();
Console.WriteLine("Press write down anything to close connection");
Console.ReadLine();
server.Shutdown();
await startServer;