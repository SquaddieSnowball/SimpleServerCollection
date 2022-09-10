using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using SimpleHttpServer.Modules.Helpers;
using SimpleTcpServer;
using System.Net;

namespace SimpleHttpServer;

/// <summary>
/// Runs a server that listens for incoming HTTP requests.
/// </summary>
public sealed class HttpServer
{
    private readonly TcpServer _tcpServer;
    private readonly Dictionary<string, Action<HttpRequest, HttpResponse>> _endPoints = new();
    private readonly HttpResponseBuilder _responseBuilder = new();

    /// <summary>
    /// Gets an IPAddress that represents the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Gets the port on which to listen for request.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the HTTP protocol versions supported by the server.
    /// </summary>
    public IEnumerable<string> SupportedProtocolVersions { get; } = new string[] { "1.0" };

    /// <summary>
    /// Initializes a new instance of the HttpServer class that runs a HTTP server
    /// which listens for HTTP requests on the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">An IPAddress that represents the local IP address.</param>
    /// <param name="port">The port on which to listen for request.</param>
    public HttpServer(IPAddress ipAddress, int port)
    {
        (IpAddress, Port) = (ipAddress, port);

        _tcpServer = new TcpServer(ipAddress, port)
        {
            RequestHandler = HandleRequest
        };
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        try
        {
            _tcpServer.Start();
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop() =>
        _tcpServer?.Stop();

    /// <summary>
    /// Adds an end point and it's handler to the HTTP server.
    /// </summary>
    /// <param name="target">Target of an end point.</param>
    /// <param name="handler">End point handler.</param>
    /// <returns>The current instance of the HttpServer class.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpServer AddEndPoint(string target, Action<HttpRequest, HttpResponse> handler)
    {
        if (string.IsNullOrEmpty(target) is true)
            throw new ArgumentException("Target must not be null or empty.",
                nameof(target));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler),
                "Handler must not be null.");

        try
        {
            _endPoints.Add(target, handler);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("The specified endpoint is already assigned.",
                nameof(target));
        }

        return this;
    }

    private IEnumerable<byte> HandleRequest(IEnumerable<byte> request)
    {
        IEnumerable<byte> response;

        HttpRequest httpRequest;
        HttpResponse httpResponse;

        try
        {
            httpRequest = HttpRequestParser.ParseFromBytes(request);
        }
        catch (ArgumentException)
        {
            httpResponse = new HttpResponse(SupportedProtocolVersions.Last(), HttpResponseStatus.BadRequest,
                Enumerable.Empty<HttpHeader>(), string.Empty);

            response = _responseBuilder.Build(httpResponse);

            return response;
        }

        if (SupportedProtocolVersions.Contains(httpRequest.ProtocolVersion) is false)
        {
            httpResponse = new HttpResponse(SupportedProtocolVersions.Last(), HttpResponseStatus.HttpVersionNotSupported,
                Enumerable.Empty<HttpHeader>(), string.Empty);

            response = _responseBuilder.Build(httpResponse);

            return response;
        }

        if (httpRequest.Method == HttpRequestMethod.NOTIMPLEMENTED)
        {
            httpResponse = new HttpResponse(httpRequest.ProtocolVersion, HttpResponseStatus.NotImplemented,
                Enumerable.Empty<HttpHeader>(), string.Empty);

            response = _responseBuilder.Build(httpResponse);

            return response;
        }

        if (_endPoints.ContainsKey(httpRequest.Target) is false)
        {
            httpResponse = new HttpResponse(httpRequest.ProtocolVersion, HttpResponseStatus.NotFound,
                Enumerable.Empty<HttpHeader>(), string.Empty);

            response = _responseBuilder.Build(httpResponse);

            return response;
        }

        httpResponse = new HttpResponse(httpRequest.ProtocolVersion, HttpResponseStatus.OK,
            Enumerable.Empty<HttpHeader>(), string.Empty);

        _endPoints[httpRequest.Target](httpRequest, httpResponse);

        response = _responseBuilder.Build(httpResponse);

        return response;
    }
}