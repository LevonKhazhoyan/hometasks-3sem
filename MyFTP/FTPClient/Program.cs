using MyFTPClient;

if (args.Length < 4)
{
    Console.WriteLine("Provide in args separated by spaces ip, port, --list/--get, path to the file or directory\n" +
                      "For example: 127.10.0.1 8888 --list \"../../../TestData/\"");
    return;
}

var parseResult = int.TryParse(args[1], out var port);
if (!parseResult)
{
    Console.WriteLine("Couldn't get port");
    return;
}

var request = args[2];
var path = args[3];
var ip = args[0];
var client = new Client(ip, port);
var cancellationToken = new CancellationTokenSource();

switch (request)
{
    case "--list":
        try
        {
            var list = await client.ListAsync(path, cancellationToken.Token);
            foreach (var response in list)
            {
                Console.WriteLine($"{response.name} {response.isDir}");
            }
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("There's no such directory");
        }
        break;
    case "--get":
    {
        using var responseStream = new MemoryStream();
        await client.GetAsync(path, responseStream, cancellationToken.Token);
        using var streamReader = new StreamReader(responseStream);
        var file = await streamReader.ReadToEndAsync();
        Console.WriteLine(file);
        break;
    }
    default:
        Console.WriteLine("Invalid request");
        break;
}