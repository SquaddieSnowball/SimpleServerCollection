using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;
using SimpleHttpServer.Extensions.Logging;
using SimpleHttpServer.Extensions.Options;
using SimpleHttpServer.Extensions.Options.Validators;
using SimpleHttpServer.Services.Abstractions;
using SimpleTcpServer;
using SimpleTcpServer.Entities;
using Validation.Helpers;

namespace SimpleHttpServer;

/// <summary>
/// Runs a server that handles incoming HTTP requests.
/// </summary>
public sealed class HttpServer
{
    private readonly TcpServer _tcpServer;

    private readonly List<HttpEndpoint> _endpoints = new();
    private readonly HttpServerOptionsValidator _optionsValidator = new();

    private readonly IOptions<HttpServerOptions> _options;
    private readonly ILogger<HttpServer> _logger;

    private readonly IHttpRequestParser _requestParser;
    private readonly IHttpResponseBuilder _responseBuilder;

    #region Properties

    /// <summary>
    /// Gets the string representing the local IP address.
    /// </summary>
    public string IpAddress => _tcpServer.IpAddress;

    /// <summary>
    /// Gets the port on which requests will be listened.
    /// </summary>
    public int Port => _tcpServer.Port;

    /// <summary>
    /// Gets the size of the byte array used as a buffer to store the request.
    /// </summary>
    public int RequestBufferSize => _tcpServer.RequestBufferSize;

    /// <summary>
    /// Gets the timeout (in milliseconds) for reading request data.
    /// </summary>
    public int RequestReadTimeout => _tcpServer.RequestReadTimeout;

    /// <summary>
    /// Gets the string representing the server name.
    /// </summary>
    public string Name => _options.Value.Name;

    /// <summary>
    /// Gets a value indicating whether the HTTP "TRACE" method is enabled on the server.
    /// </summary>
    public bool TraceEnabled => _options.Value.TraceEnabled;

    /// <summary>
    /// Gets the <see cref="IHttpServerResponseGenerator"/> assigned to the current server instance.
    /// </summary>
    public IHttpServerResponseGenerator ResponseGenerator { get; }

    /// <summary>
    /// Gets the headers attached to each server response.
    /// </summary>
    public IEnumerable<HttpHeader> DefaultHeaders =>
        new HttpHeader[]
        {
            new(HttpHeaderGroup.Response, "Server", Name),
            new(HttpHeaderGroup.Response, "Date", DateTime.Now.ToUniversalTime().ToString("R"))
        };

    /// <summary>
    /// Gets the HTTP protocol versions supported by the server.
    /// </summary>
    public static IEnumerable<string> SupportedProtocolVersions => new string[] { "1.0", "1.1" };

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the server starts.
    /// </summary>
    public event EventHandler? ServerStart;

    /// <summary>
    /// Occurs when the server is stopped.
    /// </summary>
    public event EventHandler<Exception?>? ServerStop;

    /// <summary>
    /// Occurs when the TCP connection is opened.
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
    public event EventHandler<(Exception, TcpConnection)>? TcpRequestHandlingFailAfterConnection;

    /// <summary>
    /// Occurs after the request has completed handling.
    /// </summary>
    public event EventHandler<(HttpRequest?, HttpResponse?)>? RequestHandling;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpServer"/> class that runs an HTTP server 
    /// configured with the specified options and services and logs server messages.
    /// </summary>
    /// <param name="tcpServer">An instance of the <see cref="TcpServer"/> class 
    /// that will be used to listen for incoming TCP requests.</param>
    /// <param name="options">An options instance for the server configuration.</param>
    /// <param name="logger">A logger instance that will be used to log server messages.</param>
    /// <param name="requestParser"><see cref="IHttpRequestParser"/> to parse HTTP requests.</param>
    /// <param name="responseBuilder"><see cref="IHttpResponseBuilder"/> to build HTTP responses.</param>
    /// <param name="responseGenerator"><see cref="IHttpServerResponseGenerator"/> to generate server responses.</param>
    public HttpServer(
        TcpServer tcpServer,
        IOptions<HttpServerOptions> options,
        ILogger<HttpServer> logger,
        IHttpRequestParser requestParser,
        IHttpResponseBuilder responseBuilder,
        IHttpServerResponseGenerator responseGenerator)
    {
        Verify.NotNull(tcpServer);
        Verify.NotNull(options);
        Verify.NotNull(logger);
        Verify.NotNull(requestParser);
        Verify.NotNull(responseBuilder);
        Verify.NotNull(responseGenerator);
        Verify.Options(options.Value, _optionsValidator);

        (_tcpServer, _options, _logger, _requestParser, _responseBuilder, ResponseGenerator) =
            (tcpServer, options, logger, requestParser, responseBuilder, responseGenerator);

        _tcpServer.RequestHandler = request => HandleTcpRequest(request);

        _tcpServer.ServerStart += TcpServerOnServerStart;
        _tcpServer.ServerStop += TcpServerOnServerStop;
        _tcpServer.ConnectionOpen += TcpServerOnConnectionOpen;
        _tcpServer.ConnectionClose += TcpServerOnConnectionClose;
        _tcpServer.RequestHandling += TcpServerOnRequestHandling;
        _tcpServer.RequestHandlingFailBeforeConnection += TcpServerOnRequestHandlingFailBeforeConnection;
        _tcpServer.RequestHandlingFailAfterConnection += TcpServerOnRequestHandlingFailAfterConnection;

        ResponseGenerator.Server = this;
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start() => _tcpServer.Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop() => _tcpServer.Stop();

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "GET".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapGet(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.GET, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "HEAD".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapHead(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.HEAD, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "POST".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapPost(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.POST, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "PUT".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapPut(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.PUT, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "PATCH".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapPatch(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.PATCH, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "DELETE".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapDelete(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.DELETE, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "CONNECT".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapConnect(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.CONNECT, handler);

    /// <summary>
    /// Adds a request handler for the specified HTTP target and method "OPTIONS".
    /// </summary>
    /// <param name="target">Target of the request.</param>
    /// <param name="handler">Request handler.</param>
    /// <returns>The current instance of the <see cref="HttpServer"/> class.</returns>
    public HttpServer MapOptions(string target, Func<HttpRequest, HttpResponse> handler) =>
        AddEndpoint(target, HttpRequestMethod.OPTIONS, handler);

    private HttpServer AddEndpoint(string target, HttpRequestMethod requestMethod, Func<HttpRequest, HttpResponse> handler)
    {
        Verify.NotNullOrEmpty(target);

        HttpEndpoint? endpoint = _endpoints.FirstOrDefault(e => e.Target.Equals(target, StringComparison.Ordinal));

        if (endpoint is null)
        {
            endpoint = new HttpEndpoint(target);
            _endpoints.Add(endpoint);
        }

        endpoint.AddRequestMethodHandler(requestMethod, handler);

        return this;
    }

    #region TCP server event handlers

    private void TcpServerOnServerStart(object? sender, EventArgs e)
    {
        ServerStart?.Invoke(this, e);

        _logger.LogServerStart(IpAddress, Port);
    }

    private void TcpServerOnServerStop(object? sender, Exception? e)
    {
        ServerStop?.Invoke(this, e);

        if (e is null)
            _logger.LogServerStop(IpAddress, Port);
        else
            _logger.LogServerStopException(e, IpAddress, Port);
    }

    private void TcpServerOnConnectionOpen(object? sender, TcpConnection e)
    {
        TcpConnectionOpen?.Invoke(this, e);

        _logger.LogTcpConnectionOpen(e.RemoteEndPoint.Address.ToString(), e.RemoteEndPoint.Port, e.Id);
    }

    private void TcpServerOnConnectionClose(object? sender, TcpConnection e)
    {
        TcpConnectionClose?.Invoke(this, e);

        _logger.LogTcpConnectionClose(e.Id);
    }

    private void TcpServerOnRequestHandling(object? sender, (TcpRequest, TcpResponse?) e)
    {
        TcpRequestHandling?.Invoke(this, e);

        _logger.LogTcpRequestHandling(e.Item1.Body.Count(), e.Item2?.Body?.Count() ?? default, e.Item1.Connection.Id);
    }

    private void TcpServerOnRequestHandlingFailBeforeConnection(object? sender, Exception e)
    {
        TcpRequestHandlingFailBeforeConnection?.Invoke(this, e);

        _logger.LogTcpRequestHandlingFail(e);
    }

    private void TcpServerOnRequestHandlingFailAfterConnection(object? sender, (Exception, TcpConnection) e)
    {
        TcpRequestHandlingFailAfterConnection?.Invoke(this, e);

        _logger.LogRequestHandlingFail(e.Item1.InnerException?.Message ?? e.Item1.Message, e.Item2.Id);
    }

    #endregion

    private TcpResponse HandleTcpRequest(TcpRequest tcpRequest)
    {
        TcpResponse tcpResponse;

        if ((ValidateTcpRequest(tcpRequest, out HttpRequest? httpRequest, out HttpResponse? httpResponse) is false) ||
            (ValidateHttpRequest(httpRequest!, out httpResponse) is false) ||
            (HandleHttpRequest(httpRequest!, out httpResponse) is true))
            tcpResponse = GenerateTcpResponse(httpRequest, httpResponse!, false);
        else
        {
            httpResponse = _endpoints
                .First(e => e.Target.Equals(httpRequest!.Target, StringComparison.Ordinal) is true)
                .RequestMethodHandlerCollection.First(mh => mh.Key.Equals(httpRequest!.Method) is true)
                .Value(httpRequest!);

            tcpResponse =
                (httpResponse is null) ?
                GenerateTcpResponse(
                    httpRequest,
                    ResponseGenerator.GenerateResponse(HttpResponseStatus.InternalServerError),
                    false) :
                GenerateTcpResponse(
                    httpRequest,
                    httpResponse,
                    true);
        }

        RequestHandling?.Invoke(this, (httpRequest, httpResponse));

        _logger.LogRequestHandling(
            (httpRequest is null) ?
                "INVALID" :
                $"{httpRequest.Method} {httpRequest.Target} HTTP/{httpRequest.ProtocolVersion}",
            (httpResponse is null) ?
                "INVALID" :
                $"{(int)httpResponse.Status} {httpResponse.Status}",
            tcpRequest.Connection.Id);

        return tcpResponse;
    }

    private TcpResponse GenerateTcpResponse(HttpRequest? httpRequest, HttpResponse httpResponse, bool attachGeneralHttpHeaders)
    {
        if (attachGeneralHttpHeaders is true)
            AttachGeneralHttpResponseHeaders(httpResponse);

        bool keepConnectionAlive = ManageHttpConnection(httpRequest, httpResponse);

        return new TcpResponse(_responseBuilder.Build(httpRequest, httpResponse))
        {
            KeepConnectionAlive = keepConnectionAlive
        };
    }

    private bool ValidateTcpRequest(TcpRequest tcpRequest, out HttpRequest? httpRequest, out HttpResponse? httpResponse)
    {
        httpRequest = default;
        httpResponse = default;

        try
        {
            httpRequest = _requestParser.Parse(tcpRequest.Body);
        }
        catch (ArgumentException)
        {
            httpResponse = ResponseGenerator.GenerateResponse(HttpResponseStatus.BadRequest);

            return false;
        }

        return true;
    }

    private bool ValidateHttpRequest(HttpRequest httpRequest, out HttpResponse? httpResponse) =>
        (ValidateGeneralHttpRequest(httpRequest, out httpResponse) is true) &&
        ((httpRequest.ProtocolVersion is not "1.1") || (ValidateHttp11Request(httpRequest, out httpResponse) is true));

    private bool ValidateGeneralHttpRequest(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        httpResponse = default;

        if (SupportedProtocolVersions.Contains(httpRequest.ProtocolVersion) is false)
            httpResponse = ResponseGenerator.GenerateResponse(HttpResponseStatus.HttpVersionNotSupported);
        else if (httpRequest.Method is HttpRequestMethod.NOTIMPLEMENTED)
            httpResponse = ResponseGenerator.GenerateResponse(HttpResponseStatus.NotImplemented);

        return httpResponse is null;
    }

    private bool ValidateHttp11Request(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        httpResponse = default;

        IEnumerable<HttpHeader> hostHeaders = httpRequest.Headers.Where(h => h.Parameter is "Host");
        IEnumerable<HttpHeader> connectionHeaders = httpRequest.Headers.Where(h => h.Parameter is "Connection");

        if ((hostHeaders.Count() is not 1) || (connectionHeaders.Count() is not 1) ||
            (connectionHeaders.First().Value is not ("keep-alive" or "close")))
            httpResponse = ResponseGenerator.GenerateResponse(HttpResponseStatus.BadRequest);

        return httpResponse is null;
    }

    private bool HandleHttpRequest(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        if (HandleHttpServerRequest(httpRequest, out httpResponse) is false)
        {
            HttpEndpoint? endpoint = _endpoints
                .FirstOrDefault(e => e.Target.Equals(httpRequest.Target, StringComparison.Ordinal) is true);

            if (endpoint is null)
            {
                httpResponse = ResponseGenerator
                    .GenerateResponse(HttpResponseStatus.NotFound);
            }
            else if (endpoint.RequestMethodHandlerCollection.Any(mh => mh.Key.Equals(httpRequest.Method) is true) is false)
            {
                httpResponse = ResponseGenerator
                    .GenerateResponse(
                        HttpResponseStatus.MethodNotAllowed,
                        new HttpHeader[]
                        {
                            new(
                                HttpHeaderGroup.Response,
                                "Allow",
                                string.Join(", ", endpoint.RequestMethodHandlerCollection.Select(mh => mh.Key)))
                        });
            }
        }

        return httpResponse is not null;
    }

    private bool HandleHttpServerRequest(HttpRequest httpRequest, out HttpResponse? httpResponse)
    {
        httpResponse = default;

        if ((httpRequest.Method is HttpRequestMethod.OPTIONS) && (httpRequest.Target is "*"))
            httpResponse = HandleHttpServerOptionsRequest();
        else if (httpRequest.Method is HttpRequestMethod.TRACE)
            httpResponse = HandleHttpServerTraceRequest(httpRequest);

        return httpResponse is not null;
    }

    private HttpResponse HandleHttpServerOptionsRequest() =>
        new(
            HttpResponseStatus.OK,
            DefaultHeaders
                .Concat(new HttpHeader[]
                {
                    new(
                        HttpHeaderGroup.Response,
                        "Allow",
                        (_endpoints.Any() is true) ?
                        string.Join(
                            ", ",
                            _endpoints
                                .Select(e => e.RequestMethodHandlerCollection.Select(mh => mh.Key))
                                .Aggregate((me1, me2) => me1.Concat(me2))
                                .Distinct()
                                .OrderBy(m => m)) :
                        string.Empty)
                }));

    private HttpResponse HandleHttpServerTraceRequest(HttpRequest httpRequest) =>
        (TraceEnabled is true) ?
        new HttpResponse(
            HttpResponseStatus.OK,
            DefaultHeaders
                .Concat(new HttpHeader[]
                    {
                        new(HttpHeaderGroup.Representation, "Content-Type", "message/http"),
                        new(HttpHeaderGroup.Payload, "Content-Length", httpRequest.RawRequest.Count().ToString())
                    }),
            httpRequest.RawRequest) :
        ResponseGenerator.GenerateResponse(HttpResponseStatus.NotImplemented);

    private void AttachGeneralHttpResponseHeaders(HttpResponse httpResponse) =>
        httpResponse.Headers = DefaultHeaders
            .Concat(new HttpHeader[]
            {
                new(HttpHeaderGroup.Payload, "Content-Length", (httpResponse.Body?.Count() ?? default).ToString())
            })
            .UnionBy(httpResponse.Headers ?? Enumerable.Empty<HttpHeader>(), h => h.Parameter);

    private static bool ManageHttpConnection(HttpRequest? httpRequest, HttpResponse httpResponse)
    {
        bool keepConnectionAlive = false;

        HttpHeader? connectionResponseHeader = httpResponse.Headers.FirstOrDefault(h => h.Parameter is "Connection");

        if (connectionResponseHeader is not null)
            keepConnectionAlive = connectionResponseHeader.Value is "keep-alive";
        else
        {
            HttpHeader? connectionRequestHeader = httpRequest?.Headers.FirstOrDefault(h => h.Parameter is "Connection");

            if (connectionRequestHeader is not null)
            {
                keepConnectionAlive = connectionRequestHeader.Value is "keep-alive";

                httpResponse.Headers = httpResponse.Headers
                    .Concat(new HttpHeader[]
                    {
                        new(HttpHeaderGroup.Response, "Connection", (keepConnectionAlive is true) ? "keep-alive" : "close")
                    });
            }
        }

        return keepConnectionAlive;
    }
}