using Microsoft.Extensions.Logging;

namespace SimpleHttpServer.Extensions.Logging;

/// <summary>
/// Provides methods for logging HTTP server messages.
/// </summary>
internal static partial class LogHttpServerMessages
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
        this ILogger<HttpServer> logger,
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
        this ILogger<HttpServer> logger,
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
        this ILogger<HttpServer> logger,
        Exception stopException,
        string ipAddress,
        int port);

    /// <summary>
    /// Logs a message indicating that a TCP connection to the server is open.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="remoteIpAddress">IP address of the remote endpoint.</param>
    /// <param name="remotePort">Remote endpoint port.</param>
    /// <param name="connectionId">Connection ID.</param>
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 2000,
        Message = "[{RemoteIpAddress}:{RemotePort}] - TCP connection is open [cid:{ConnectionId}]")]
    public static partial void LogTcpConnectionOpen(
        this ILogger<HttpServer> logger,
        string remoteIpAddress,
        int remotePort,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that the TCP connection to the server is closed.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="connectionId">Connection ID.</param>
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 2001,
        Message = "TCP connection closed [cid:{ConnectionId}]")]
    public static partial void LogTcpConnectionClose(
        this ILogger<HttpServer> logger,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that the TCP request to the server has been handled.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="requestSize">Request size.</param>
    /// <param name="responseSize">Response size.</param>
    /// <param name="connectionId">ID of the connection through which the request came.</param>
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 3000,
        Message = "TCP request handled: Received - {RequestSize} bytes, sent - {ResponseSize} bytes [cid:{ConnectionId}]")]
    public static partial void LogTcpRequestHandling(
        this ILogger<HttpServer> logger,
        int requestSize,
        int responseSize,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that handling a TCP request to the server failed.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="requestHandlingException">An exception due to which request handling failed.</param>
    [LoggerMessage(
        Level = LogLevel.Critical,
        EventId = 3001,
        Message = "TCP request handling failed")]
    public static partial void LogTcpRequestHandlingFail(
        this ILogger<HttpServer> logger,
        Exception requestHandlingException);

    /// <summary>
    /// Logs a message indicating that the request to the server has been handled.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="requestInfo">Information describing the request.</param>
    /// <param name="responseInfo">Information describing the response.</param>
    /// <param name="connectionId">ID of the connection through which the request came.</param>
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 4000,
        Message = "Request handled: Request - \"{RequestInfo}\", response - \"{ResponseInfo}\" [cid:{ConnectionId}]")]
    public static partial void LogRequestHandling(
        this ILogger<HttpServer> logger,
        string requestInfo,
        string responseInfo,
        Guid connectionId);

    /// <summary>
    /// Logs a message indicating that handling a request to the server failed.
    /// </summary>
    /// <param name="logger">Server logger.</param>
    /// <param name="failureInfo">Information describing the failure.</param>
    /// <param name="connectionId">ID of the connection through which the request came.</param>
    [LoggerMessage(
        Level = LogLevel.Warning,
        EventId = 4001,
        Message = "Request handling failed: \"{FailureInfo}\" [cid:{ConnectionId}]")]
    public static partial void LogRequestHandlingFail(
        this ILogger<HttpServer> logger,
        string failureInfo,
        Guid connectionId);
}