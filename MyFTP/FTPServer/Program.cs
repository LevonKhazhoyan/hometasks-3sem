using MyFTPServer;

if (args.Length < 2)
{
    Console.WriteLine("Expected ip and port");
    return;
}

var ip = args[0];
var port = int.Parse(args[1]);
var server = new Server(ip, port);
var startServer = server.StartAsync();
Console.WriteLine("Press enter to stop");
Console.ReadLine();
server.Shutdown();
await startServer;