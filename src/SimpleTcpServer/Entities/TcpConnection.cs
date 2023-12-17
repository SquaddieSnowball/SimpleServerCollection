using System.Net;

namespace SimpleTcpServer.Entities;

/// <summary>
/// Represents a TCP connection.
/// </summary>
/// <param name="Id">Connection ID.</param>
/// <param name="RemoteEndPoint">The endpoint to which the connection was established.</param>
public record class TcpConnection(Guid Id, IPEndPoint RemoteEndPoint);