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
    private readonly Dictionary<string, Action<HttpRequest, HttpResponse>> _endpoints = new();

    /// <summary>
    /// Gets an IPAddress that represents the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Gets the port on which to listen for request.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets maximum HTTP protocol version supported by the server.
    /// </summary>
    public string ProtocolVersion { get; } = "1.2";

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
    public void Stop() => _tcpServer?.Stop();

    /// <summary>
    /// Adds an endpoint and it's handler to the HTTP server.
    /// </summary>
    /// <param name="target">Endpoint to add to the HTTP server.</param>
    /// <param name="handler">Endpoint handler.</param>
    /// <returns>The current instance of the HttpServer class.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpServer AddEndpoint(string target, Action<HttpRequest, HttpResponse> handler)
    {
        if (string.IsNullOrEmpty(target) is true)
            throw new ArgumentException("Target must not be null or empty.", nameof(target));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler), "Handler must not be null.");

        try
        {
            _endpoints.Add(target, handler);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("The specified endpoint is already assigned.", nameof(target));
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
            httpResponse = new HttpResponse(ProtocolVersion, HttpResponseStatus.BadRequest,
                Enumerable.Empty<HttpHeader>(), string.Empty);

            response = HttpResponseBuilder.Build(httpResponse);

            return response;
        }

        if (httpRequest.Method == HttpRequestMethod.NOTIMPLEMENTED)
        {
            httpResponse = new HttpResponse(ProtocolVersion, HttpResponseStatus.NotImplemented,
                Enumerable.Empty<HttpHeader>(), string.Empty);

            response = HttpResponseBuilder.Build(httpResponse);

            return response;
        }

        foreach (KeyValuePair<string, Action<HttpRequest, HttpResponse>> endpoint in _endpoints)
            if (endpoint.Key.Equals(httpRequest.Target, StringComparison.Ordinal))
            {
                httpResponse = new HttpResponse(ProtocolVersion, HttpResponseStatus.OK,
                    Enumerable.Empty<HttpHeader>(), string.Empty);

                endpoint.Value(httpRequest, httpResponse);

                response = HttpResponseBuilder.Build(httpResponse);

                return response;
            }

        httpResponse = new HttpResponse(ProtocolVersion, HttpResponseStatus.NotFound,
            Enumerable.Empty<HttpHeader>(), string.Empty);

        response = HttpResponseBuilder.Build(httpResponse);

        return response;
    }
}