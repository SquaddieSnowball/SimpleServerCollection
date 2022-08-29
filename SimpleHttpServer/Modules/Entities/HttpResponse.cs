using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Entities;

/// <summary>
/// Represents an HTTP response.
/// </summary>
public sealed class HttpResponse
{
    /// <summary>
    /// Gets or sets protocol version of an HTTP response.
    /// </summary>
    public string ProtocolVersion { get; set; }

    /// <summary>
    /// Gets or sets status of an HTTP response.
    /// </summary>
    public HttpResponseStatus Status { get; set; }

    /// <summary>
    /// Gets or sets headers of an HTTP response.
    /// </summary>
    public IEnumerable<HttpHeader> Headers { get; set; }

    /// <summary>
    /// Gets or sets body of an HTTP response.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Initializes a new instance of the HttpResponse class.
    /// </summary>
    /// <param name="protocolVersion">Protocol version of an HTTP response.</param>
    /// <param name="status">Status of an HTTP response.</param>
    /// <param name="headers">Headers of an HTTP response.</param>
    /// <param name="body">Body of an HTTP response.</param>
    public HttpResponse(string protocolVersion, HttpResponseStatus status, IEnumerable<HttpHeader> headers, string body) =>
        (ProtocolVersion, Status, Headers, Body) = (protocolVersion, status, headers, body);
}