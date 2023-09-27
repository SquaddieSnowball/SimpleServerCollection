namespace SimpleTcpServer.Modules.Entities;

/// <summary>
/// Represents a TCP request.
/// </summary>
/// <param name="Body">Request body.</param>
/// <param name="Connection">The connection through which the request came.</param>
public record class TcpRequest(IEnumerable<byte> Body, TcpConnection Connection);