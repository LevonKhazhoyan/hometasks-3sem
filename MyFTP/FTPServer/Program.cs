using MyFTPServer;

if (args.Length is < 1 or > 3)
{
    Console.WriteLine("Provide in args separated by spaces: ip, port\n" +
                      "If you want to listen to all available network interfaces on the local machine provide in just the port");
    return;
}

var server = args.Length == 2 ? new Server(int.Parse(args[1]), args[0]) : new Server(int.Parse(args[1]));
var startServer = server.StartAsync();
Console.WriteLine("Write down anything to close connection");
Console.ReadLine();
server.Shutdown();
await startServer;