using Microsoft.Extensions.Logging;
using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using SimpleHttpServer.Modules.Helpers;
using SimpleHttpServer.Modules.Services;
using SimpleTcpServer;
using SimpleTcpServer.Modules.Entities;
using System.Net;
using System.Net.Sockets;

namespace SimpleHttpServer;

/// <summary>
/// Runs a server that listens for incoming HTTP requests.
/// </summary>
public sealed class HttpServer
{
    private readonly TcpServer _tcpServer;
    private readonly List<HttpEndpoint> _endpoints = new();

    private readonly HttpServerResponseGenerator _responseGenerator;
    private readonly ILogger<HttpServer>? _logger;

    /// <summary>
    /// Gets the <see cref="IPAddress"/> representing the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Gets the port on which requests will be listened.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the server name.
    /// </summary>
    public string Name { get; } = "Simple HTTP server";

    /// <summary>
    /// Gets the HTTP protocol versions supported by the server.
    /// </summary>
    public IEnumerable<string> SupportedProtocolVersions { get; } = new string[] { "1.0", "1.1" };

    /// <summary>
    /// Gets or sets the amount of time the server waits for a request over an open connection.
    /// </summary>
    public int ConnectionTimeout
    {
        get => _tcpServer.RequestReadTimeout;
        set
        {
            if (value is < 1 or > 86400)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "The connection timeout value must be in the range of 1 to 86400.");
            }

            _tcpServer.RequestReadTimeout = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the HTTP "TRACE" method is enabled on the server.
    /// </summary>
    public bool IsTraceEnabled { get; set; }

    /// <summary>
    /// Occurs when the server starts.
    /// </summary>
    public event EventHandler? ServerStart;

    /// <summary>
    /// Occurs when the server is stopped.
    /// </summary>
    public event EventHandler<Exception?>? ServerStop;

    /// <summary>
    /// Occurs when a TCP connection is opened.
    /// </summary>
    public event EventHandler<TcpConnection>? TcpConnectionOpen;

    /// <summary>
    /// Occurs when the TCP connection is closed.
    /// </summary>
    public event EventHandler<TcpConnection>? TcpConnectionClose;

    /// <summary>
    /// Occurs after the TCP request has completed handling.
    /// </summary>
    public event EventHandler<(TcpRequest, TcpResponse?)>? TcpRequestHandling;

    /// <summary>
    /// Occurs when TCP request handling fails before a connection is opened.
    /// </summary>
    public event EventHandler<Exception>? TcpRequestHandlingFailBeforeConnection;

    /// <summary>
    /// Occurs when TCP request handling fails after the connection is opened.
    /// </summary>
    public event EventHandler<(TcpConnection, Exception)>? TcpRequestHandlingFailAfterConnection;

    /// <summary>
    /// Occurs after the HTTP request has completed handling.
    /// </summary>
    public event EventHandler<(HttpRequest?, HttpResponse?)>? HttpRequestHandling;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpServer"/> class, that runs an HTTP server, 
    /// that listens for requests at the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress"><see cref="IPAddress"/> representing the local IP address.</param>
    /// <param name="port">The port on which requests will be listened to.</param>
    public HttpServer(IPAddress ipAddress, int port)
    {
        (_tcpServer, _responseGenerator, IpAddress, Port) = (
            new TcpServer(ipAddress, port)
            {
                RequestReadTimeout = 60000,
                RequestHandler = HandleTcpRequest
            },
            new HttpServerResponseGenerator(this),
            ipAddress,
            port);

        ManageTcpServerEvents();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpServer"/> class, that runs an HTTP server, 
    /// that listens for requests at the specified local IP address and port number and logs server events.
    /// </summary>
    /// <param name="ipAddress"><see cref="IPAddress"/> representing the local IP address.</param>
    /// <param name="port">The port on which requests will be listened to.</param>
    /// <param name="logger">A logger that will be used to log server events.</param>
    public HttpServer(IPAddress ipAddress, int port, ILogger<HttpServer> logger) : this(ipAddress, port) => _logger = logger;

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
    public void Stop() => _tcpServer.Stop();

    /// <summary>
    /// Maps the target and request handler for the HTTP "GET" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapGet(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.GET, handler);

    /// <summary>
    /// Maps the target and request handler for the HTTP "HEAD" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapHead(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.HEAD, handler);

    /// <summary>
    /// Maps the target and request handler for the HTTP "POST" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapPost(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.POST, handler);

    /// <summary>
    /// Maps the target and request handler for the HTTP "PUT" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapPut(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.PUT, handler);

    /// <summary>
    /// Maps the target and request handler for the HTTP "PATCH" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapPatch(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.PATCH, handler);

    /// <summary>
    /// Maps the target and request handler for the HTTP "DELETE" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapDelete(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.DELETE, handler);

    /// <summary>
    /// Maps the target and request handler for the HTTP "OPTIONS" method.
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapOptions(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.OPTIONS, handler);

    private HttpServer AddEndpoint(string target, HttpRequestMethod requestMethod, Func<HttpRequest, HttpResponse> handler)
    {
        if (string.IsNullOrEmpty(target) is true)
            throw new ArgumentException("The target must not be null or empty.", nameof(target));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler), "The handler must not be null.");

        HttpEndpoint? endpoint = _endpoints.FirstOrDefault(e => e.Target.Equals(target, StringComparison.Ordinal));

        if (endpoint is null)
        {
            endpoint = new HttpEndpoint(target);
            _endpoints.Add(endpoint);
        }

        try
        {
            endpoint.AddRequestMethodHandler(requestMethod, handler);
        }
        catch
        {
            throw;
        }

        return this;
    }

    private void ManageTcpServerEvents()
    {
        _tcpServer.ServerStart += TcpServerOnServerStart;
        _tcpServer.ServerStop += TcpServerOnServerStop;
        _tcpServer.ConnectionOpen += TcpServerOnConnectionOpen;
        _tcpServer.ConnectionClose += TcpServerOnConnectionClose;
        _tcpServer.RequestHandling += TcpServerOnRequestHandling;
        _tcpServer.RequestHandlingFailBeforeConnection += TcpServerOnRequestHandlingFailBeforeConnection;
        _tcpServer.RequestHandlingFailAfterConnection += TcpServerOnRequestHandlingFailAfterConnection;
    }

    private void TcpServerOnServerStart(object? sender, EventArgs e)
    {
        ServerStart?.Invoke(this, e);

        _logger?.LogInformation(LoggingEvents.ServerStart,
            "{IpAddress}:{Port} - {EventName}",
            IpAddress, Port, LoggingEvents.ServerStart.Name);
    }

    private void TcpServerOnServerStop(object? sender, Exception? e)
    {
        ServerStop?.Invoke(this, e);

        if (e is null)
        {
            _logger?.LogInformation(LoggingEvents.ServerStop,
                "{IpAddress}:{Port} - {EventName}",
                IpAddress, Port, LoggingEvents.ServerStop.Name);
        }
        else
        {
            _logger?.LogCritical(LoggingEvents.ServerStopException,
                e,
                "{IpAddress}:{Port} - {EventName}",
                IpAddress, Port, LoggingEvents.ServerStopException.Name);
        }
    }

    private void TcpServerOnConnectionOpen(object? sender, TcpConnection e)
    {
        TcpConnectionOpen?.Invoke(this, e);

        _logger?.LogInformation(LoggingEvents.TcpConnectionOpen,
            "{TcpConnectionId} - {EventName}",
            e.Id, LoggingEvents.TcpConnectionOpen.Name);
    }

    private void TcpServerOnConnectionClose(object? sender, TcpConnection e)
    {
        TcpConnectionClose?.Invoke(this, e);

        _logger?.LogInformation(LoggingEvents.TcpConnectionClose,
            "{TcpConnectionId} - {EventName}",
            e.Id, LoggingEvents.TcpConnectionClose.Name);
    }

    private void TcpServerOnRequestHandling(object? sender, (TcpRequest, TcpResponse?) e)
    {
        TcpRequestHandling?.Invoke(this, e);

        _logger?.LogInformation(LoggingEvents.TcpRequestHandling,
            "{TcpConnectionId} - {EventName}. Bytes received: {TcpRequestSize}, bytes sent: {TcpResponseSize}",
            e.Item1.Connection.Id, LoggingEvents.TcpRequestHandling.Name, e.Item1.Body.Count(), e.Item2?.Body?.Count() ?? 0);
    }

    private void TcpServerOnRequestHandlingFailBeforeConnection(object? sender, Exception e)
    {
        TcpRequestHandlingFailBeforeConnection?.Invoke(this, e);

        _logger?.LogCritical(LoggingEvents.TcpRequestHandlingFailBeforeConnection,
            e,
            "{EventName}",
            LoggingEvents.TcpRequestHandlingFailBeforeConnection.Name);
    }

    private void TcpServerOnRequestHandlingFailAfterConnection(object? sender, (TcpConnection, Exception) e)
    {
        TcpRequestHandlingFailAfterConnection?.Invoke(this, (e.Item1, e.Item2));

        switch ((e.Item2.InnerException as SocketException)?.SocketErrorCode)
        {
            case SocketError.TimedOut:

                _logger?.LogWarning(LoggingEvents.TcpRequestHandlingFailAfterConnection,
                    "{TcpConnectionId} - {EventName}: \"Connection timed out\"",
                    e.Item1.Id, LoggingEvents.TcpRequestHandlingFailAfterConnection.Name);

                break;
            case SocketError.ConnectionReset:

                _logger?.LogWarning(LoggingEvents.TcpRequestHandlingFailAfterConnection,
                    "{TcpConnectionId} - {EventName}: \"Connection closed by remote host\"",
                    e.Item1.Id, LoggingEvents.TcpRequestHandlingFailAfterConnection.Name);

                break;
            default:

                _logger?.LogError(LoggingEvents.TcpRequestHandlingFailAfterConnection,
                    e.Item2,
                    "{TcpConnectionId} - {EventName}",
                    e.Item1.Id, LoggingEvents.TcpRequestHandlingFailAfterConnection.Name);

                break;
        }
    }

    private TcpResponse HandleTcpRequest(TcpRequest tcpRequest)
    {
        TcpResponse tcpResponse;

        if ((ValidateTcpRequest(tcpRequest, out HttpRequest? httpRequest, out HttpResponse? httpResponse) is false) ||
            (ValidateHttpRequest(httpRequest!, out httpResponse) is false) ||
            (HandleHttpRequest(httpRequest!, out httpResponse) is true))
            tcpResponse = new TcpResponse(HttpResponseBuilder.Build(httpResponse!));
        else
        {
            httpResponse = _endpoints
                .First(e => e.Target.Equals(httpRequest!.Target, StringComparison.Ordinal) is true)
                .RequestMethodHandlerCollection.First(mh => mh.Key.Equals(httpRequest!.Method) is true)
                .Value(httpRequest!);

            if (httpResponse is null)
                tcpResponse = new TcpResponse(Array.Empty<byte>());
            else
            {
                SupplementHttpResponse(httpRequest!, httpResponse);

                bool keepConnectionAlive = ManageHttpConnection(httpRequest!, httpResponse);

                tcpResponse = new TcpResponse(HttpResponseBuilder.Build(httpResponse))
                {
                    KeepConnectionAlive = keepConnectionAlive
                };
            }
        }

        HttpRequestHandling?.Invoke(this, (httpRequest, httpResponse));

        _logger?.LogInformation(LoggingEvents.HttpRequestHandling,
            "{TcpConnectionId} - {EventName}. Request info: {HttpRequestInfo}, response info: {HttpResponseInfo}",
            tcpRequest.Connection.Id,
            LoggingEvents.HttpRequestHandling.Name,
            (httpRequest is null) ?
                "\"Invalid HTTP request\"" :
                $"\"{httpRequest.Method} {httpRequest.Target} HTTP/{httpRequest.ProtocolVersion}\"",
            (httpResponse is null) ?
                "\"Invalid HTTP response\"" :
                $"\"HTTP/{httpResponse.ProtocolVersion} {(int)httpResponse.Status} {httpResponse.Status}\"");

        return tcpResponse;
    }

    private bool ValidateTcpRequest(TcpRequest tcpRequest, out HttpRequest? httpRequest, out HttpResponse? httpResponse)
    {
        httpRequest = default;
        httpResponse = default;

        try
        {
            httpRequest = HttpRequestParser.ParseFromBytes(tcpRequest.Body);
        }
        catch (ArgumentException)
        {
            httpResponse = _responseGenerator.GenerateBadRequest();

            return false;
        }

        return true;
    }

    private bool ValidateHttpRequest(HttpRequest httpRequest, out HttpResponse? httpResponse) =>
        (ValidateHttpRequestGeneral(httpRequest, out httpResponse) is true) &&
        ((httpRequest.ProtocolVersion is not "1.1") || (ValidateHttpRequest11(httpRequest, out httpResponse) is true));

    private bool ValidateHttpRequestGeneral(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        httpResponse = default;

        if (SupportedProtocolVersions.Contains(httpRequest.ProtocolVersion) is false)
            httpResponse = _responseGenerator.GenerateHttpVersionNotSupported();
        else if (httpRequest.Method is HttpRequestMethod.NOTIMPLEMENTED)
            httpResponse = _responseGenerator.GenerateNotImplemented(httpRequest);

        return httpResponse is null;
    }

    private bool ValidateHttpRequest11(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        httpResponse = default;

        IEnumerable<HttpHeader> hostHeaders = httpRequest.Headers.Where(h => h.Parameter is "Host");
        IEnumerable<HttpHeader> connectionHeaders = httpRequest.Headers.Where(h => h.Parameter is "Connection");

        if ((hostHeaders.Count() is not 1) || (connectionHeaders.Count() is not 1) ||
            (connectionHeaders.First().Value is not ("keep-alive" or "close")))
            httpResponse = _responseGenerator.GenerateBadRequest(httpRequest);

        return httpResponse is null;
    }

    private bool HandleHttpRequest(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        if (HandleHttpServerRequest(httpRequest, out httpResponse) is false)
        {
            HttpEndpoint? endpoint = _endpoints
                .FirstOrDefault(e => e.Target.Equals(httpRequest.Target, StringComparison.Ordinal) is true);

            if (endpoint is null)
                httpResponse = _responseGenerator.GenerateNotFound(httpRequest);
            else if (endpoint.RequestMethodHandlerCollection.Any(mh => mh.Key.Equals(httpRequest.Method) is true) is false)
            {
                httpResponse = _responseGenerator
                    .GenerateMethodNotAllowed(httpRequest, endpoint.RequestMethodHandlerCollection.Select(mh => mh.Key));
            }
        }

        return httpResponse is not null;
    }

    private bool HandleHttpServerRequest(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        httpResponse = default;

        if ((httpRequest.Method is HttpRequestMethod.OPTIONS) && (httpRequest.Target is "*"))
            httpResponse = HandleHttpOptionsRequest(httpRequest);
        else if (httpRequest.Method is HttpRequestMethod.TRACE)
            httpResponse = HandleHttpTraceRequest(httpRequest);

        return httpResponse is not null;
    }

    private HttpResponse HandleHttpOptionsRequest(HttpRequest httpRequest) =>
        new(
            httpRequest.ProtocolVersion,
            HttpResponseStatus.OK,
            _responseGenerator.GeneralServerHeaders.Concat(new HttpHeader[]
            {
                new HttpHeader(
                    HttpHeaderGroup.Response,
                    "Allow",
                    (_endpoints.Any() is true) ?
                    string.Join(", ", _endpoints
                        .Select(e => e.RequestMethodHandlerCollection.Select(mh => mh.Key))
                        .Aggregate((me1, me2) => me1.Concat(me2))
                        .Distinct()
                        .OrderBy(m => m)) :
                    string.Empty)
            }));

    private HttpResponse HandleHttpTraceRequest(HttpRequest httpRequest) =>
        (IsTraceEnabled is true) ?
        new HttpResponse(
            httpRequest.ProtocolVersion,
            HttpResponseStatus.OK,
            _responseGenerator.GeneralServerHeaders.Concat(new HttpHeader[]
            {
                new HttpHeader(
                    HttpHeaderGroup.Representation,
                    "Content-Type",
                    "message/http"),
                new HttpHeader(
                    HttpHeaderGroup.Payload,
                    "Content-Length",
                    httpRequest.RawRequest.Length.ToString())
            }),
            httpRequest.RawRequest) :
        _responseGenerator.GenerateNotImplemented(httpRequest);

    private void SupplementHttpResponse(HttpRequest httpRequest, HttpResponse httpResponse)
    {
        if (string.IsNullOrEmpty(httpResponse.ProtocolVersion) is true)
            httpResponse.ProtocolVersion = httpRequest.ProtocolVersion;

        httpResponse.Headers ??= new List<HttpHeader>();

        IEnumerable<HttpHeader> generalResponseHeaders = _responseGenerator.GeneralServerHeaders.Concat(new HttpHeader[]
        {
            new HttpHeader(HttpHeaderGroup.Payload, "Content-Length", (httpResponse.Body?.Length ?? 0).ToString())
        });

        List<HttpHeader> responseHeadersToAttach = new();

        foreach (HttpHeader responseHeader in generalResponseHeaders)
        {
            if (httpResponse.Headers.Any(h => h.Parameter?.Equals(responseHeader.Parameter, StringComparison.Ordinal) is true))
                continue;

            responseHeadersToAttach.Add(responseHeader);
        }

        httpResponse.Headers = httpResponse.Headers.Concat(responseHeadersToAttach);
    }

    private static bool ManageHttpConnection(HttpRequest httpRequest, HttpResponse httpResponse)
    {
        bool keepConnectionAlive = false;

        HttpHeader? connectionHeader = httpRequest.Headers.FirstOrDefault(h => h.Parameter is "Connection");

        if (connectionHeader is not null)
        {
            keepConnectionAlive = connectionHeader.Value is "keep-alive";

            httpResponse.Headers = httpResponse.Headers.Concat(new HttpHeader[]
            {
                new HttpHeader(
                    HttpHeaderGroup.Response,
                    "Connection",
                    (keepConnectionAlive is true) ? "keep-alive" : "close")
            });
        }

        return keepConnectionAlive;
    }
}