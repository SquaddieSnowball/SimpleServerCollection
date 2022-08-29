using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Entities;

public sealed class HttpResponse
{
    public string ProtocolVersion { get; set; }

    public HttpResponseStatus Status { get; set; }

    public IEnumerable<HttpHeader> Headers { get; set; }

    public string Body { get; set; }

    public HttpResponse(string protocolVersion, HttpResponseStatus status, IEnumerable<HttpHeader> headers, string body) =>
        (ProtocolVersion, Status, Headers, Body) = (protocolVersion, status, headers, body);
}