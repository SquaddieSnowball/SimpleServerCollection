using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Entities;

/// <summary>
/// Represents an HTTP response.
/// </summary>
/// <param name="ProtocolVersion">Protocol version of the response.</param>
/// <param name="Status">Response status.</param>
/// <param name="Headers">Response headers.</param>
/// <param name="Body">Response body.</param>
public record class HttpResponse(string ProtocolVersion, HttpResponseStatus Status,
    IEnumerable<HttpHeader> Headers, string? Body = default)
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
    public string? Body { get; set; } = Body;
}