using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleTcpServer.Entities;
using SimpleTcpServer.Extensions.Logging;
using SimpleTcpServer.Extensions.Options;
using SimpleTcpServer.Extensions.Options.Validators;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Validation.Helpers;

namespace SimpleTcpServer;

/// <summary>
/// Runs a server that listens for requests from TCP network clients.
/// </summary>
public sealed class TcpServer
{
    private readonly TcpListener _server;

    private readonly ConcurrentDictionary<Guid, TcpClient> _connectedClients = new();
    private readonly TcpServerOptionsValidator _optionsValidator = new();

    private readonly IOptions<TcpServerOptions> _options;
    private readonly ILogger<TcpServer> _logger;

    private bool _serverStarted;

    #region Properties

    /// <summary>
    /// Gets the string representing the local IP address.
    /// </summary>
    public string IpAddress => _options.Value.IpAddress!;

    /// <summary>
    /// Gets the port on which requests will be listened.
    /// </summary>
    public int Port => _options.Value.Port!.Value;

    /// <summary>
    /// Gets the size of the byte array used as a buffer to store the request.
    /// </summary>
    public int RequestBufferSize => _options.Value.RequestBufferSize;

    /// <summary>
    /// Gets the timeout (in milliseconds) for reading request data.
    /// </summary>
    public int RequestReadTimeout => _options.Value.RequestReadTimeout;

    /// <summary>
    /// The method used to handle the request.
    /// </summary>
    public Func<TcpRequest, TcpResponse>? RequestHandler { get; set; }

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
    /// Occurs when the connection is opened.
    /// </summary>
    public event EventHandler<TcpConnection>? ConnectionOpen;

    /// <summary>
    /// Occurs when the connection is closed.
    /// </summary>
    public event EventHandler<TcpConnection>? ConnectionClose;

    /// <summary>
    /// Occurs after the request has completed handling.
    /// </summary>
    public event EventHandler<(TcpRequest, TcpResponse?)>? RequestHandling;

    /// <summary>
    /// Occurs when request handling fails before a connection is opened.
    /// </summary>
    public event EventHandler<Exception>? RequestHandlingFailBeforeConnection;

    /// <summary>
    /// Occurs when request handling fails after the connection is opened.
    /// </summary>
    public event EventHandler<(Exception, TcpConnection)>? RequestHandlingFailAfterConnection;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TcpServer"/> class that runs a TCP server 
    /// configured with the specified options and logs server messages.
    /// </summary>
    /// <param name="options">An options instance for the server configuration.</param>
    /// <param name="logger">A logger instance that will be used to log server messages.</param>
    public TcpServer(IOptions<TcpServerOptions> options, ILogger<TcpServer> logger)
    {
        Verify.NotNull(options);
        Verify.NotNull(logger);
        Verify.Options(options.Value, _optionsValidator);

        (_options, _logger) = (options, logger);

        _server = new TcpListener(IPAddress.Parse(IpAddress), Port);
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        if (_serverStarted is true)
            return;

        try
        {
            _server.Start();
            _serverStarted = true;

            ServerStart?.Invoke(this, EventArgs.Empty);

            _logger.LogServerStart(IpAddress, Port);
        }
        catch
        {
            throw;
        }

        AcceptConnection();
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        if (_serverStarted is false)
            return;

        Exception? stopException = default;

        try
        {
            _server.Stop();
        }
        catch (Exception ex)
        {
            stopException = ex;
        }
        finally
        {
            _serverStarted = false;

            ServerStop?.Invoke(this, stopException);

            if (stopException is null)
                _logger.LogServerStop(IpAddress, Port);
            else
                _logger.LogServerStopException(stopException, IpAddress, Port);
        }
    }

    /// <summary>
    /// Closes the connection to the server.
    /// </summary>
    /// <param name="connection">Connection to close.</param>
    /// <returns><see langword="true"/> if the connection was closed; otherwise, <see langword="false"/>.</returns>
    public bool CloseConnection(TcpConnection connection)
    {
        Verify.NotNull(connection);

        try
        {
            _connectedClients[connection.Id].Close();

            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
        catch
        {
            throw;
        }
    }

    private void AcceptConnection()
    {
        try
        {
            _ = _server.BeginAcceptTcpClient(HandleConnection, _server);
        }
        catch (Exception ex)
        {
            RequestHandlingFailBeforeConnection?.Invoke(this, ex);

            _logger.LogRequestHandlingFailBeforeConnection(ex);

            Stop();
        }
    }

    private void HandleConnection(IAsyncResult result)
    {
        if (_serverStarted is false)
            return;

        AcceptConnection();

        TcpClient client = _server.EndAcceptTcpClient(result);

        TcpConnection connection = new(Guid.NewGuid(), (IPEndPoint)client.Client.RemoteEndPoint!);
        _ = _connectedClients.TryAdd(connection.Id, client);

        ConnectionOpen?.Invoke(this, connection);

        _logger.LogConnectionOpen(connection.RemoteEndPoint.Address.ToString(), connection.RemoteEndPoint.Port, connection.Id);

        try
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = RequestReadTimeout;

            HandleRequest(connection, stream);
        }
        catch (Exception ex)
        {
            RequestHandlingFailAfterConnection?.Invoke(this, (ex, connection));

            _logger.LogRequestHandlingFailAfterConnection(ex, connection.Id);
        }
        finally
        {
            client.Close();
            _ = _connectedClients.TryRemove(connection.Id, out _);

            ConnectionClose?.Invoke(this, connection);

            _logger.LogConnectionClose(connection.Id);
        }
    }

    private void HandleRequest(TcpConnection connection, NetworkStream stream)
    {
        IEnumerable<byte> requestBody = Enumerable.Empty<byte>();

        do
        {
            byte[] requestBuffer = new byte[RequestBufferSize];
            int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
            requestBody = requestBody.Concat(requestBuffer.Take(bytesRead));
        }
        while (stream.DataAvailable is true);

        TcpRequest request = new(requestBody, connection);
        TcpResponse? response = RequestHandler?.Invoke(request);

        byte[] responseBytes = (response?.Body?.Any() is true) ? response.Body.ToArray() : new byte[] { default };
        stream.Write(responseBytes, 0, responseBytes.Length);

        RequestHandling?.Invoke(this, (request, response));

        _logger.LogRequestHandling(requestBody.Count(), responseBytes.Length, connection.Id);

        if (response?.KeepConnectionAlive is true)
            HandleRequest(connection, stream);
    }
}