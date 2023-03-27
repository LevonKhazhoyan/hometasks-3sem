using MyFTPServer;

if (args.Length < 2)
{
    Console.WriteLine("Expected ip and port");
    return;
}

var server = new Server(args[0], int.Parse(args[1]));
var startServer = server.StartAsync();
Console.WriteLine("Press enter to stop");
Console.ReadLine();
server.Shutdown();
await startServer;