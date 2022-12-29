namespace SimpleTcpServer.Modules.Entities;

/// <summary>
/// Represents a TCP request.
/// </summary>
public sealed class TcpRequest
{
    /// <summary>
    /// Gets the connection through which the request came.
    /// </summary>
    public TcpConnection Connection { get; }

    /// <summary>
    /// Gets the body of the TCP request.
    /// </summary>
    public IEnumerable<byte> Body { get; }

    /// <summary>
    /// Initializes a new instance of the TcpRequest class.
    /// </summary>
    /// <param name="connectionId">The connection through which the request came.</param>
    /// <param name="body">The body of the TCP request.</param>
    public TcpRequest(TcpConnection connection, IEnumerable<byte> body) =>
        (Connection, Body) = (connection, body);
}