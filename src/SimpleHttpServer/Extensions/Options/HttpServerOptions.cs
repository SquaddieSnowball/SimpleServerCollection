using System.ComponentModel.DataAnnotations;

namespace SimpleHttpServer.Extensions.Options;

/// <summary>
/// Represents HTTP server options.
/// </summary>
public sealed class HttpServerOptions
{
    /// <summary>
    /// Gets or sets the server name.
    /// </summary>
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "Simple HTTP Server";

    /// <summary>
    /// Gets or sets a value indicating whether the HTTP "TRACE" method is enabled on the server.
    /// </summary>
    public bool TraceEnabled { get; set; }
}