using Microsoft.Extensions.Logging;
using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using SimpleHttpServer.Modules.Helpers;
using SimpleHttpServer.Modules.Services;
using SimpleTcpServer;
using SimpleTcpServer.Modules.Entities;
using System.Net;

namespace SimpleHttpServer;

/// <summary>
/// Runs a server that listens for incoming HTTP requests.
/// </summary>
public sealed class HttpServer
{
    private readonly TcpServer _tcpServer;
    private readonly ILogger<HttpServer>? _logger;
    private readonly Dictionary<string, Action<HttpRequest, HttpResponse>> _endPoints = new();
    private readonly Dictionary<Guid, Timer> _activeConnections = new();
    private readonly object _lock = new();
    private int _connectionTimeout = 60000;
    private ResponseGenerator? _responseGenerator;

    /// <summary>
    /// Gets an IPAddress that represents the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Gets the port on which to listen for request.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets the name of the server.
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
        get => _connectionTimeout;
        set
        {
            if (value is < 1 or > 86400)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "The connection timeout value must be between 1 and 86400.");

            _connectionTimeout = value * 1000;
        }
    }

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
            RequestHandler = HandleTcpRequest
        };

        SubscribeToTcpServerEvents();
    }

    /// <summary>
    /// Initializes a new instance of the HttpServer class that runs a HTTP server
    /// which listens for HTTP requests on the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">An IPAddress that represents the local IP address.</param>
    /// <param name="port">The port on which to listen for request.</param>
    /// <param name="logger">The logger that will be used to log events.</param>
    public HttpServer(IPAddress ipAddress, int port, ILogger<HttpServer> logger) : this(ipAddress, port) =>
        _logger = logger;

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
        _tcpServer.Stop();

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

    private void SubscribeToTcpServerEvents()
    {
        _tcpServer.ServerStart += (sender, e) =>
            _logger?.LogInformation(LoggingEvents.ServerStart, "{EventName} at {IpAddress}:{Port}",
                LoggingEvents.ServerStart.Name, IpAddress, Port);

        _tcpServer.ServerStop += (sender, e) =>
        {
            if (e is null)
                _logger?.LogInformation(LoggingEvents.ServerStop, "{EventName} at {IpAddress}:{Port}",
                    LoggingEvents.ServerStop.Name, IpAddress, Port);
            else
                _logger?.LogCritical(LoggingEvents.ServerStopException, e, "{EventName} at {IpAddress}:{Port}",
                    LoggingEvents.ServerStopException.Name, IpAddress, Port);
        };

        _tcpServer.ConnectionOpen += (sender, e) =>
            _logger?.LogInformation(LoggingEvents.ConnectionOpen, "{EventName} on {IpAddress}:{Port} (ID: {ConnectionId})",
                LoggingEvents.ConnectionOpen.Name, e.RemoteEndPoint.Address,
                e.RemoteEndPoint.Port, e.Id);

        _tcpServer.ConnectionClose += (sender, e) =>
        {
            lock (_lock)
            {
                if (_activeConnections.ContainsKey(e.Id) is true)
                {
                    _activeConnections[e.Id].Dispose();
                    _ = _activeConnections.Remove(e.Id);
                }
            }

            _logger?.LogInformation(LoggingEvents.ConnectionClose, "{EventName} (ID: {ConnectionId})",
                LoggingEvents.ConnectionClose.Name, e.Id);
        };

        _tcpServer.RequestHandling += (sender, e) =>
            _logger?.LogInformation(LoggingEvents.RequestHandling, "{EventName} (Conndection ID: {ConnectionId})",
                LoggingEvents.RequestHandling.Name, e.Connection.Id);

        _tcpServer.RequestHandlingFailBeforeConnection += (sender, e) =>
            _logger?.LogCritical(LoggingEvents.RequestHandlingFailBeforeConnection, e,
                "{EventName}", LoggingEvents.RequestHandlingFailBeforeConnection.Name);

        _tcpServer.RequestHandlingFailAfterConnection += (sender, e) =>
            _logger?.LogError(LoggingEvents.RequestHandlingFailAfterConnection, e.Item1,
                "{EventName} (Conndection ID: {ConnectionId})",
                LoggingEvents.RequestHandlingFailAfterConnection.Name, e.Item2.Id);
    }

    private TcpResponse HandleTcpRequest(TcpRequest tcpRequest)
    {
        if (tcpRequest.Body.Any() is false)
            return new TcpResponse(Array.Empty<byte>());

        _responseGenerator ??= new(this);

        HttpRequest httpRequest;
        HttpResponse httpResponse;

        TcpResponse? tcpResponse;

        try
        {
            httpRequest = HttpRequestParser.ParseFromBytes(tcpRequest.Body);
        }
        catch (ArgumentException)
        {
            return _responseGenerator.GenerateBadRequestResponse();
        }

        tcpResponse = ValidateHttpRequest(httpRequest);

        if (tcpResponse is not null)
            return tcpResponse;

        httpResponse = new HttpResponse(httpRequest.ProtocolVersion, HttpResponseStatus.OK, Enumerable.Empty<HttpHeader>());

        tcpResponse = httpRequest.Method switch
        {
            HttpRequestMethod.GET => HandleHttpGetRequest(httpRequest, httpResponse),
            HttpRequestMethod.HEAD => HandleHttpHeadRequest(httpRequest, httpResponse),
            _ => _responseGenerator.GenerateNotImplementedResponse(httpRequest)
        };

        ManageConnection(httpResponse, tcpRequest, tcpResponse);

        return tcpResponse;
    }

    private TcpResponse? ValidateHttpRequest(HttpRequest httpRequest)
    {
        HttpHeader? hostHeader =
            httpRequest.Headers.FirstOrDefault(h => h.Parameter.Equals("Host", StringComparison.Ordinal));
        HttpHeader? connectionHeader =
            httpRequest.Headers.FirstOrDefault(h => h.Parameter.Equals("Connection", StringComparison.Ordinal));

        if (httpRequest.Method is HttpRequestMethod.NOTIMPLEMENTED)
            return _responseGenerator!.GenerateNotImplementedResponse(httpRequest);

        if (SupportedProtocolVersions.Contains(httpRequest.ProtocolVersion) is false)
            return _responseGenerator!.GenerateHttpVersionNotSupportedResponse();

        if ((hostHeader is null) && (httpRequest.ProtocolVersion is not "1.0"))
            return _responseGenerator!.GenerateBadRequestResponse();

        if ((connectionHeader is null) && (httpRequest.ProtocolVersion is not "1.0"))
            return _responseGenerator!.GenerateBadRequestResponse();

        if ((connectionHeader is not null) && (connectionHeader.Value is not ("keep-alive" or "close")))
            return _responseGenerator!.GenerateBadRequestResponse();

        if (_endPoints.ContainsKey(httpRequest.Target) is false)
            return _responseGenerator!.GenerateNotFoundResponse(httpRequest);

        return default;
    }

    private TcpResponse HandleHttpGetRequest(HttpRequest httpRequest, HttpResponse httpResponse)
    {
        _endPoints[httpRequest.Target](httpRequest, httpResponse);
        _responseGenerator!.AttachResponseHeaders(httpResponse, httpRequest);

        return new TcpResponse(HttpResponseBuilder.Build(httpResponse));
    }

    private TcpResponse HandleHttpHeadRequest(HttpRequest httpRequest, HttpResponse httpResponse)
    {
        _endPoints[httpRequest.Target](httpRequest, httpResponse);
        _responseGenerator!.AttachResponseHeaders(httpResponse, httpRequest);
        httpResponse.Body = default;

        return new TcpResponse(HttpResponseBuilder.Build(httpResponse));
    }

    private void ManageConnection(HttpResponse httpResponse, TcpRequest tcpRequest, TcpResponse tcpResponse)
    {
        HttpHeader? connectionHeader =
            httpResponse.Headers.FirstOrDefault(h => h.Parameter.Equals("Connection", StringComparison.Ordinal) is true);

        if ((connectionHeader is not null) && (connectionHeader.Value.Equals("keep-alive", StringComparison.Ordinal) is true))
        {
            lock (_lock)
            {
                if (_activeConnections.ContainsKey(tcpRequest.Connection.Id) is true)
                    _ = _activeConnections[tcpRequest.Connection.Id].Change(_connectionTimeout, Timeout.Infinite);
                else
                    _activeConnections.Add(tcpRequest.Connection.Id,
                        new Timer(state => _ = _tcpServer.CloseConnection((Guid)state!),
                        tcpRequest.Connection.Id, _connectionTimeout, Timeout.Infinite));
            }

            tcpResponse.KeepConnectionAlive = true;
        }
    }
}