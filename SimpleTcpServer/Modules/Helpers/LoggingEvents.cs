using Microsoft.Extensions.Logging;

namespace SimpleTcpServer.Modules.Helpers;

/// <summary>
/// Represents a type containing logging event IDs.
/// </summary>
internal static class LoggingEvents
{
    public static EventId ServerStart = new(1000, "Server is running");
    public static EventId ServerStop = new(1001, "Server stopped");
    public static EventId ServerStopException = new(1002, "Server stopped with exception");
    public static EventId ConnectionOpen = new(2000, "Connection is open");
    public static EventId ConnectionClose = new(2001, "Connection closed");
    public static EventId RequestHandling = new(3000, "Request handled");
    public static EventId RequestHandlingFailBeforeConnection = new(3001, "Request handling failed (before connection)");
    public static EventId RequestHandlingFailAfterConnection = new(3002, "Request handling failed (after connection)");
}