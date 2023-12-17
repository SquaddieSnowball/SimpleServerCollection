using Microsoft.Extensions.Logging;

namespace SimpleTcpServer.Extensions.Logging;

/// <summary>
/// Provides methods for logging TCP server messages.
/// </summary>
internal static partial class LogTcpServerMessages
{
    /// <summary>
    /// Logs a message indicating that the server is running.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="ipAddress">Server IP address.</param>
    /// <param name="port">Server port.</param>
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 1000,
        Message = "[{IpAddress}:{Port}] - Server is running")]
    public static partial void LogServerStart(
        this ILogger<TcpServer> logger,
        string ipAddress,
        int port);

    /// <summary>
    /// Logs a message indicating that the server has stopped.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="ipAddress">Server IP address.</param>
    /// <param name="port">Server port.</param>
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 1001,
        Message = "[{IpAddress}:{Port}] - Server stopped")]
    public static partial void LogServerStop(
        this ILogger<TcpServer> logger,
        string ipAddress,
        int port);

    /// <summary>
    /// Logs a message indicating that the server has stopped with an exception.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="stopException">Exception thrown before the server stopped.</param>
    /// <param name="ipAddress">Server IP address.</param>
    /// <param name="port">Server port.</param>
    [LoggerMessage(
        Level = LogLevel.Critical,
        EventId = 1002,
        Message = "[{IpAddress}:{Port}] - Server stopped with exception")]
    public static partial void LogServerStopException(
        this ILogger<TcpServer> logger,
        Exception stopException,
        string ipAddress,
        int port);

    /// <summary>
    /// Logs a message indicating that a connection to the server is open.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="remoteIpAddress">IP address of the remote endpoint.</param>
    /// <param name="remotePort">Remote endpoint port.</param>
    /// <param name="connectionId">Connection ID.</param>
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 2000,
        Message = "[{RemoteIpAddress}:{RemotePort}] - Connection is open [cid:{ConnectionId}]")]
    public static partial void LogConnectionOpen(
        this ILogger<TcpServer> logger,
        string remoteIpAddress,
        int remotePort,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that the connection to the server is closed.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="connectionId">Connection ID.</param>
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 2001,
        Message = "Connection closed [cid:{ConnectionId}]")]
    public static partial void LogConnectionClose(
        this ILogger<TcpServer> logger,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that the request to the server has been handled.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="requestSize">Request size.</param>
    /// <param name="responseSize">Response size.</param>
    /// <param name="connectionId">ID of the connection through which the request came.</param>
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 3000,
        Message = "Request handled: Received - {RequestSize} bytes, sent - {ResponseSize} bytes [cid:{ConnectionId}]")]
    public static partial void LogRequestHandling(
        this ILogger<TcpServer> logger,
        int requestSize,
        int responseSize,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that handling a request to the server failed before the connection was established.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="requestHandlingException">An exception due to which request handling failed.</param>
    [LoggerMessage(
        Level = LogLevel.Critical,
        EventId = 3001,
        Message = "Request handling failed (before connection)")]
    public static partial void LogRequestHandlingFailBeforeConnection(
        this ILogger<TcpServer> logger,
        Exception requestHandlingException);

    /// <summary>
    /// Logs a message indicating that handling a request to the server failed after the connection was established.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="requestHandlingException">An exception due to which request handling failed.</param>
    /// <param name="connectionId">ID of the connection through which the request came.</param>
    [LoggerMessage(
        Level = LogLevel.Error,
        EventId = 3002,
        Message = "Request handling failed (after connection) [cid:{ConnectionId}]")]
    public static partial void LogRequestHandlingFailAfterConnection(
        this ILogger<TcpServer> logger,
        Exception requestHandlingException,
        Guid connectionId);
}