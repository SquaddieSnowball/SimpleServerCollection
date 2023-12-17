using System.ComponentModel.DataAnnotations;
using Validation.Attributes;

namespace SimpleTcpServer.Extensions.Options;

/// <summary>
/// Represents TCP server options.
/// </summary>
public sealed class TcpServerOptions
{
    /// <summary>
    /// Gets or sets the string representing the local IP address.
    /// </summary>
    [Required]
    [IpAddress]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the port on which requests will be listened.
    /// </summary>
    [Required]
    [Range(0, 65535)]
    public int? Port { get; set; }

    /// <summary>
    /// Gets or sets the size of the byte array used as a buffer to store the request.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int RequestBufferSize { get; set; } = 1024;

    /// <summary>
    /// Gets or sets the timeout (in milliseconds) for reading request data.
    /// </summary>
    [RangeExcept(-1, int.MaxValue, 0)]
    public int RequestReadTimeout { get; set; } = Timeout.Infinite;
}