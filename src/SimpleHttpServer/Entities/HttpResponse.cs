using SimpleHttpServer.Entities.Components;

namespace SimpleHttpServer.Entities;

/// <summary>
/// Represents an HTTP response.
/// </summary>
/// <param name="Status">Response status.</param>
/// <param name="Headers">Response headers.</param>
/// <param name="Body">Response body.</param>
public record class HttpResponse(HttpResponseStatus Status, IEnumerable<HttpHeader> Headers, IEnumerable<byte>? Body = default)
{
    /// <summary>
    /// Response status.
    /// </summary>
    public HttpResponseStatus Status { get; set; } = Status;

    /// <summary>
    /// Response headers.
    /// </summary>
    public IEnumerable<HttpHeader> Headers { get; set; } = Headers;

    /// <summary>
    /// Response body.
    /// </summary>
    public IEnumerable<byte>? Body { get; set; } = Body;
}