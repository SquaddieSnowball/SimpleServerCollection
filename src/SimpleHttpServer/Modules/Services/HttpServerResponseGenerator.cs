using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Services;

/// <summary>
/// Represents a service for generating HTTP server responses.
/// </summary>
public sealed class HttpServerResponseGenerator
{
    private readonly HttpServer _httpServer;

    /// <summary>
    /// Gets the general HTTP server headers.
    /// </summary>
    public IEnumerable<HttpHeader> GeneralServerHeaders => new List<HttpHeader>()
    {
        new HttpHeader(HttpHeaderGroup.Response, "Server", _httpServer.Name),
        new HttpHeader(HttpHeaderGroup.Response, "Date", DateTime.Now.ToUniversalTime().ToString("R"))
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpServerResponseGenerator"/> class, 
    /// which is used to generate HTTP server responses.
    /// </summary>
    /// <param name="httpServer">HTTP server for which requests will be generated.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpServerResponseGenerator(HttpServer httpServer)
    {
        if (httpServer is null)
            throw new ArgumentNullException(nameof(httpServer), "The HTTP server must not be null.");

        _httpServer = httpServer;
    }

    /// <summary>
    /// Generates a "Bad Request" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateBadRequest(HttpRequest? httpRequest = default) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.BadRequest,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage400.Length.ToString())
        }),
        Properties.Resources.ResponsePage400);

    /// <summary>
    /// Generates an "Unauthorized" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateUnauthorized(HttpRequest httpRequest) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.Unauthorized,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage401.Length.ToString())
        }),
        Properties.Resources.ResponsePage401);

    /// <summary>
    /// Generates a "Forbidden" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateForbidden(HttpRequest httpRequest) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.Forbidden,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage403.Length.ToString())
        }),
        Properties.Resources.ResponsePage403);

    /// <summary>
    /// Generates a "Not Found" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateNotFound(HttpRequest httpRequest) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.NotFound,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage404.Length.ToString())
        }),
        Properties.Resources.ResponsePage404);

    /// <summary>
    /// Generates a "Method Not Allowed" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <param name="allowedMethods">The set of allowed HTTP methods.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateMethodNotAllowed(HttpRequest httpRequest, IEnumerable<HttpRequestMethod> allowedMethods) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.MethodNotAllowed,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage405.Length.ToString()),
            new HttpHeader(
                HttpHeaderGroup.Response,
                "Allow",
                string.Join(", ", allowedMethods?.OrderBy(m => m) ?? Enumerable.Empty<HttpRequestMethod>()))
        }),
        Properties.Resources.ResponsePage405);

    /// <summary>
    /// Generates an "Internal Server Error" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateInternalServerError(HttpRequest httpRequest) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.InternalServerError,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage500.Length.ToString())
        }),
        Properties.Resources.ResponsePage500);

    /// <summary>
    /// Generates a "Not Implemented" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateNotImplemented(HttpRequest httpRequest) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.NotImplemented,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage501.Length.ToString())
        }),
        Properties.Resources.ResponsePage501);

    /// <summary>
    /// Generates a "Service Unavailable" HTTP response.
    /// </summary>
    /// <param name="httpRequest">An HTTP request to retrieve additional information about the response.</param>
    /// <param name="retryDateTime">The date and time at which the request can be repeated.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateServiceUnavailable(HttpRequest httpRequest, DateTime retryDateTime) => new(
        httpRequest?.ProtocolVersion ?? _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.ServiceUnavailable,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage503.Length.ToString()),
            new HttpHeader(
                HttpHeaderGroup.Response,
                "Retry-After",
                retryDateTime.ToUniversalTime().ToString("R"))
        }),
        Properties.Resources.ResponsePage503);

    /// <summary>
    /// Generates an "HTTP Version Not Supported" HTTP response.
    /// </summary>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateHttpVersionNotSupported() => new(
        _httpServer.SupportedProtocolVersions.Last(),
        HttpResponseStatus.HttpVersionNotSupported,
        GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(
                HttpHeaderGroup.Representation,
                "Content-Type",
                "text/html; charset=utf-8"),
            new HttpHeader(
                HttpHeaderGroup.Payload,
                "Content-Length",
                Properties.Resources.ResponsePage505.Length.ToString())
        }),
        Properties.Resources.ResponsePage505);
}