using Microsoft.Extensions.Logging;

namespace SimpleTcpServer.Modules.Helpers;

/// <summary>
/// Represents a type that contains logging events IDs.
/// </summary>
internal static class LoggingEvents
{
    internal static EventId ServerStart = new(1000, "Server started");
    internal static EventId ServerStop = new(1001, "Server stopped");
    internal static EventId ServerStopException = new(1002, "Server stopped with exception");
    internal static EventId ConnectionOpen = new(2000, "Connection opened");
    internal static EventId ConnectionClose = new(2001, "Connection closed");
    internal static EventId RequestHandling = new(3000, "Request handled");
    internal static EventId RequestHandlingFailBeforeConnection = new(3001, "Request handling failed before connection");
    internal static EventId RequestHandlingFailAfterConnection = new(3002, "Request handling failed after connection");
}