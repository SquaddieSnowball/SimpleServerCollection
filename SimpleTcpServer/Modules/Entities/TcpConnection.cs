using System.Net;

namespace SimpleTcpServer.Modules.Entities;

/// <summary>
/// Represents a TCP connection.
/// </summary>
public sealed class TcpConnection
{
    /// <summary>
    /// Gets the connection ID.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the endpoint to which the connection is established.
    /// </summary>
    public IPEndPoint RemoteEndPoint { get; }

    /// <summary>
    /// Initializes a new instance of the TcpConnection class.
    /// </summary>
    /// <param name="id">The connection ID.</param>
    /// <param name="remoteEndPoint">The endpoint to which the connection is established.</param>
    public TcpConnection(Guid id, IPEndPoint remoteEndPoint) =>
        (Id, RemoteEndPoint) = (id, remoteEndPoint);
}