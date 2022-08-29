using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Entities;

/// <summary>
/// Represents an HTTP request.
/// </summary>
public sealed class HttpRequest
{
    /// <summary>
    /// Method of an HTTP request.
    /// </summary>
    public HttpRequestMethod Method { get; }

    /// <summary>
    /// Target of an HTTP request.
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Query parameters of an HTTP request.
    /// </summary>
    public IEnumerable<HttpRequestQueryParameter> QueryParameters { get; }

    /// <summary>
    /// Protocol version of an HTTP request.
    /// </summary>
    public string ProtocolVersion { get; }

    /// <summary>
    /// Headers of an HTTP request.
    /// </summary>
    public IEnumerable<HttpHeader> Headers { get; }

    /// <summary>
    /// Body of an HTTP request.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// Initializes a new instance of the HttpRequest class.
    /// </summary>
    /// <param name="method">Method of an HTTP request.</param>
    /// <param name="target">Target of an HTTP request.</param>
    /// <param name="queryParameters">Query parameters of an HTTP request.</param>
    /// <param name="protocolVersion">Protocol version of an HTTP request.</param>
    /// <param name="headers">Headers of an HTTP request.</param>
    /// <param name="body">Body of an HTTP request.</param>
    public HttpRequest(HttpRequestMethod method, string target, IEnumerable<HttpRequestQueryParameter> queryParameters,
        string protocolVersion, IEnumerable<HttpHeader> headers, string body) =>
        (Method, Target, QueryParameters, ProtocolVersion, Headers, Body) =
        (method, target, queryParameters, protocolVersion, headers, body);
}