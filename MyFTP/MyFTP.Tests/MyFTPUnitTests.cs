namespace MyFTP.Tests;

using MyFTPClient;
using MyFTPServer;
using System.Net;
using NUnit.Framework.Constraints;
using NUnit.Framework;

[TestFixture]
public class Tests
{
    private Client client;
    private Server server;
    private readonly IPAddress ip = IPAddress.Parse("127.0.0.1");
    private const int port = 8888;
    private const string path = "../../../TestData/";
    private readonly CancellationTokenSource cancellationToken = new();

    [SetUp]
    public void SetupAsync()
    {
        server = new Server(ip, port);
        client = new Client(ip, port);
        _ = server.StartAsync();
    }

    [TearDown]
    public void Teardown()
        => server.Shutdown();

    [Test]
    public void IncorrectPathGetShouldThrowException()
    {
        Assert.Throws(new ResolvableConstraintExpression().InnerException.TypeOf<FileNotFoundException>(), 
            () => client.GetAsync("IncorrectPath", new MemoryStream(), cancellationToken.Token).Wait());
    }

    [Test]
    public void IncorrectPathListShouldThrowException()
    {
        Assert.Throws(new ResolvableConstraintExpression().InnerException.TypeOf<DirectoryNotFoundException>(),
            () => client.ListAsync("IncorrectPath", cancellationToken.Token).Wait());
    }
    
    [TestCase(path + "DonaldByrneVSBobbyFischer.txt")]
    [TestCase(path + "TestDirectory/KasparovVSDeepBlue№1.txt")]
    public async Task GetTest(string filePath)
    {
        using var stream = new MemoryStream();
        await client.GetAsync(filePath, stream, cancellationToken.Token);
        using var streamReader = new StreamReader(stream);
        var file = await streamReader.ReadToEndAsync();

        await using var fileStream = File.OpenRead(filePath);
        Assert.That(fileStream, Is.Not.Null);
        using var reader = new StreamReader(fileStream);
        var answerFile = await reader.ReadToEndAsync();
        Assert.That(answerFile, Is.EqualTo(file));
    }

    [TestCase(path)]
    [TestCase(path + "TestDirectory/")]
    public async Task ListTest(string filePath)
    {
        var directory = new DirectoryInfo(filePath);
        var directories = directory.GetDirectories();
        var files = directory.GetFiles();
        var response = await client.ListAsync(filePath, cancellationToken.Token);
        Assert.That(files.Length + directories.Length, Is.EqualTo(response.Count));

        var fileCount = 0;
        foreach (var file in files)
        {
            Assert.That((filePath + file.Name, false), Is.EqualTo(response[fileCount]));
            fileCount++;
        }
        foreach (var folder in directories)
        {
            Assert.That((filePath + folder.Name, true), Is.EqualTo(response[fileCount]));
            fileCount++;
        }
    }
}