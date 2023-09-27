namespace SimpleTcpServer.Modules.Entities;

/// <summary>
/// Represents a TCP response.
/// </summary>
/// <param name="Body">Response body.</param>
public record class TcpResponse(IEnumerable<byte> Body)
{
    /// <summary>
    /// Response body.
    /// </summary>
    public IEnumerable<byte> Body { get; set; } = Body;

    /// <summary>
    /// A value that determines whether the connection should be kept alive after the request has been processed.
    /// </summary>
    public bool KeepConnectionAlive { get; set; }
}