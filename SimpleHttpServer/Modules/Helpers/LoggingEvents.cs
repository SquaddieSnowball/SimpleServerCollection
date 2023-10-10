using Microsoft.Extensions.Logging;

namespace SimpleHttpServer.Modules.Helpers;

/// <summary>
/// Represents a type containing logging event IDs.
/// </summary>
internal static class LoggingEvents
{
    public static EventId ServerStart = new(1000, "Server is running");
    public static EventId ServerStop = new(1001, "Server stopped");
    public static EventId ServerStopException = new(1002, "Server stopped with exception");
    public static EventId TcpConnectionOpen = new(2000, "TCP connection is open");
    public static EventId TcpConnectionClose = new(2001, "TCP connection closed");
    public static EventId TcpRequestHandling = new(3000, "TCP request handled");
    public static EventId TcpRequestHandlingFailBeforeConnection = new(3001, "TCP request handling failed (before connection)");
    public static EventId TcpRequestHandlingFailAfterConnection = new(3002, "TCP request handling failed (after connection)");
    public static EventId HttpRequestHandling = new(4000, "HTTP request handled");
}