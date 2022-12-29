using SimpleHttpServer.Modules.Entities.Components;
using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Helpers;
using SimpleTcpServer.Modules.Entities;

namespace SimpleHttpServer.Modules.Services;

/// <summary>
/// Provides methods used to generate server responses.
/// </summary>
internal sealed class ResponseGenerator
{
    private readonly HttpServer _httpServer;

    /// <summary>
    /// Initializes a new instance of the ResponseGenerator class.
    /// </summary>
    /// <param name="httpServer">An instance of the HttpServer class used to generate responses.</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal ResponseGenerator(HttpServer httpServer)
    {
        if (httpServer is null)
            throw new ArgumentNullException(nameof(httpServer),
                "HTTP server must not be null.");

        _httpServer = httpServer;
    }

    /// <summary>
    /// Attaches default server headers to the HTTP response.
    /// </summary>
    /// <param name="httpResponse">The HTTP response to which the headers are to be attached.</param>
    /// <param name="httpRequest">The HTTP request to extract additional headers from.</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal void AttachResponseHeaders(HttpResponse httpResponse, HttpRequest? httpRequest = default)
    {
        if (httpResponse is null)
            throw new ArgumentNullException(nameof(httpResponse),
                "HTTP response must not be null.");

        List<HttpHeader> headers = httpResponse.Headers?.ToList() ?? new();

        _ = headers.RemoveAll(h => h.Parameter is "Server" or "Date" or "Content-Length" or "Connection");

        headers.Add(new HttpHeader(HttpHeaderGroup.Response, "Server", _httpServer.Name));
        headers.Add(new HttpHeader(HttpHeaderGroup.Response, "Date", DateTime.Now.ToUniversalTime().ToString("R")));
        headers.Add(new HttpHeader(HttpHeaderGroup.Response, "Content-Length", (httpResponse.Body?.Length ?? 0).ToString()));

        if (httpRequest is not null)
        {
            HttpHeader? connectionHeader =
                httpRequest.Headers.FirstOrDefault(h => h.Parameter.Equals("Connection", StringComparison.Ordinal) is true);

            if (connectionHeader is not null)
                headers.Add(new HttpHeader(HttpHeaderGroup.Response, connectionHeader.Parameter, connectionHeader.Value));
        }

        httpResponse.Headers = headers;
    }

    /// <summary>
    /// Generates a "Bad Request" server response.
    /// </summary>
    /// <returns>"Bad Request" server response.</returns>
    internal TcpResponse GenerateBadRequestResponse() =>
        GenerateResponse(_httpServer.SupportedProtocolVersions.Last(), HttpResponseStatus.BadRequest,
            Enumerable.Empty<HttpHeader>(), Properties.Resources.ServerResponsePage400);

    /// <summary>
    /// Generates a "Not Found" server response.
    /// </summary>
    /// <param name="httpRequest">The HTTP request to which the response should be generated.</param>
    /// <returns>"Not Found" server response.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal TcpResponse GenerateNotFoundResponse(HttpRequest httpRequest)
    {
        if (httpRequest is null)
            throw new ArgumentNullException(nameof(httpRequest),
                "HTTP request must not be null.");

        return GenerateResponse(httpRequest.ProtocolVersion, HttpResponseStatus.NotFound,
            Enumerable.Empty<HttpHeader>(), Properties.Resources.ServerResponsePage404);
    }

    /// <summary>
    /// Generates a "Not Implemented" server response.
    /// </summary>
    /// <param name="httpRequest">The HTTP request to which the response should be generated.</param>
    /// <returns>"Not Implemented" server response.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal TcpResponse GenerateNotImplementedResponse(HttpRequest httpRequest)
    {
        if (httpRequest is null)
            throw new ArgumentNullException(nameof(httpRequest),
                "HTTP request must not be null.");

        return GenerateResponse(httpRequest.ProtocolVersion, HttpResponseStatus.NotImplemented,
            Enumerable.Empty<HttpHeader>(), Properties.Resources.ServerResponsePage501);
    }

    /// <summary>
    /// Generates a "HTTP Version Not Supported" server response.
    /// </summary>
    /// <returns>"HTTP Version Not Supported" server response.</returns>
    internal TcpResponse GenerateHttpVersionNotSupportedResponse() =>
        GenerateResponse(_httpServer.SupportedProtocolVersions.Last(), HttpResponseStatus.HttpVersionNotSupported,
            Enumerable.Empty<HttpHeader>(), Properties.Resources.ServerResponsePage505);

    private TcpResponse GenerateResponse(string protocolVersion, HttpResponseStatus status,
        IEnumerable<HttpHeader> headers, string body)
    {
        HttpResponse httpResponse = new(protocolVersion, status, headers, body);

        AttachResponseHeaders(httpResponse);

        httpResponse.Headers =
            httpResponse.Headers.Concat(new HttpHeader[] { new(HttpHeaderGroup.Response, "Connection", "close") });

        return new(HttpResponseBuilder.Build(httpResponse));
    }
}