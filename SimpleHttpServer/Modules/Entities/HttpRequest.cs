using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Entities;

public sealed class HttpRequest
{
    public HttpRequestMethod Method { get; }

    public string Target { get; }

    public IEnumerable<HttpRequestQueryParameter> QueryParameters { get; }

    public string ProtocolVersion { get; }

    public IEnumerable<HttpHeader> Headers { get; }

    public string Body { get; }

    public HttpRequest(HttpRequestMethod method, string target, IEnumerable<HttpRequestQueryParameter> queryParameters,
        string protocolVersion, IEnumerable<HttpHeader> headers, string body) =>
        (Method, Target, QueryParameters, ProtocolVersion, Headers, Body) =
        (method, target, queryParameters, protocolVersion, headers, body);
}