using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using SimpleHttpServer.Modules.Helpers;
using SimpleTcpServer;
using System.Net;

namespace SimpleHttpServer;

public sealed class HttpServer
{
    private readonly TcpServer _tcpServer;
    private readonly Dictionary<string, Action<HttpRequest, HttpResponse>> _endpoints = new();

    public IPAddress IpAddress { get; }

    public int Port { get; }

    public string ProtocolVersion { get; } = "1.2";

    public HttpServer(IPAddress ipAddress, int port)
    {
        (IpAddress, Port) = (ipAddress, port);

        _tcpServer = new TcpServer(ipAddress, port)
        {
            RequestHandler = HandleRequest
        };
    }

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

    public void Stop() => _tcpServer?.Stop();

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