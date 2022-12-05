namespace SimpleTcpServer.Modules.Entities;

/// <summary>
/// Represents a TCP response.
/// </summary>
public sealed class TcpResponse
{
    /// <summary>
    /// Gets or sets a value determining whether the connection should be maintained after the request has been handled.
    /// </summary>
    public bool KeepConnectionAlive { get; set; }

    /// <summary>
    /// Gets or sets the body of the TCP response.
    /// </summary>
    public IEnumerable<byte> Body { get; set; }

    /// <summary>
    /// Initializes a new instance of the TcpResponse class.
    /// </summary>
    /// <param name="body">The body of the TCP response.</param>
    public TcpResponse(IEnumerable<byte> body) =>
        Body = body;
}