namespace MyFTPServer.Domain;

/// <summary>
/// Class that represents FTP request
/// </summary>
public class Request
{
    public RequestType Type { get; }
    public string Value { get; }

    public Request(int type, string value)
    {
        Value = value;
        Type = type switch
        {
            1 => RequestType.List,
            2 => RequestType.Get,
            _ => throw new ArgumentException("Invalid request type")
        };
    }
}