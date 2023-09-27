using Microsoft.Extensions.Logging;
using SimpleTcpServer.Modules.Entities;
using SimpleTcpServer.Modules.Helpers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace SimpleTcpServer;

/// <summary>
/// Runs a server that listens for requests from TCP network clients.
/// </summary>
public sealed class TcpServer
{
    private TcpListener? _server;
    private bool _isServerStarted;
    private int _requestBufferSize = 1024;
    private int _requestReadTimeout = Timeout.Infinite;

    private readonly ConcurrentDictionary<Guid, TcpClient> _connectedClients = new();

    private readonly ILogger<TcpServer>? _logger;

    private Exception? _stopException;
    private Exception? _requestHandlingException;

    /// <summary>
    /// Gets the IPAddress representing the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Gets the port on which requests will be listened.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets or sets the size of the byte array used as a buffer to store the request.
    /// </summary>
    public int RequestBufferSize
    {
        get => _requestBufferSize;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "The request buffer size value must be greater than 0.");

            _requestBufferSize = value;
        }
    }

    /// <summary>
    /// Gets or sets the timeout for request data.
    /// </summary>
    public int RequestReadTimeout
    {
        get => _requestReadTimeout;
        set
        {
            if (value is not Timeout.Infinite and < 1)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "The request read timeout value must be greater than 0 or \"Timeout.Infinite\".");

            _requestReadTimeout = value;
        }
    }

    /// <summary>
    /// The method used to handle the request.
    /// </summary>
    public Func<TcpRequest, TcpResponse>? RequestHandler { get; set; }

    /// <summary>
    /// Occurs when the server starts.
    /// </summary>
    public event EventHandler? ServerStart;

    /// <summary>
    /// Occurs when the server is stopped.
    /// </summary>
    public event EventHandler<Exception?>? ServerStop;

    /// <summary>
    /// Occurs when a connection is opened.
    /// </summary>
    public event EventHandler<TcpConnection>? ConnectionOpen;

    /// <summary>
    /// Occurs when the connection is closed.
    /// </summary>
    public event EventHandler<TcpConnection>? ConnectionClose;

    /// <summary>
    /// Occurs after the request has completed handling.
    /// </summary>
    public event EventHandler<TcpRequest>? RequestHandling;

    /// <summary>
    /// Occurs when request handling fails before a connection is opened.
    /// </summary>
    public event EventHandler<Exception>? RequestHandlingFailBeforeConnection;

    /// <summary>
    /// Occurs when request handling fails after the connection is opened.
    /// </summary>
    public event EventHandler<(TcpConnection, Exception)>? RequestHandlingFailAfterConnection;

    /// <summary>
    /// Initializes a new instance of the TcpServer class that runs a TCP server 
    /// that listens for requests at the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">IPAddress representing the local IP address.</param>
    /// <param name="port">The port on which requests will be listened to.</param>
    public TcpServer(IPAddress ipAddress, int port) => (IpAddress, Port) = (ipAddress, port);

    /// <summary>
    /// Initializes a new instance of the TcpServer class that runs a TCP server 
    /// that listens for requests at the specified local IP address and port number and logs server events.
    /// </summary>
    /// <param name="ipAddress">IPAddress representing the local IP address.</param>
    /// <param name="port">The port on which requests will be listened to.</param>
    /// <param name="logger">A logger that will be used to log server events.</param>
    public TcpServer(IPAddress ipAddress, int port, ILogger<TcpServer> logger) : this(ipAddress, port) => _logger = logger;

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        if (_isServerStarted is true)
            return;

        try
        {
            _server = new TcpListener(IpAddress, Port);
            _server.Start();

            _isServerStarted = true;

            ServerStart?.Invoke(this, EventArgs.Empty);

            _logger?.LogInformation(LoggingEvents.ServerStart,
                "{IpAddress}:{Port} - {EventName}",
                IpAddress, Port, LoggingEvents.ServerStart.Name);
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
        if (_isServerStarted is false)
            return;

        _stopException = default;

        try
        {
            _server!.Stop();
        }
        catch (Exception ex)
        {
            _stopException = ex;
        }
        finally
        {
            _isServerStarted = false;

            ServerStop?.Invoke(this, _stopException);

            if (_stopException is null)
                _logger?.LogInformation(LoggingEvents.ServerStop,
                    "{IpAddress}:{Port} - {EventName}",
                    IpAddress, Port, LoggingEvents.ServerStop.Name);
            else
                _logger?.LogCritical(LoggingEvents.ServerStopException,
                    _stopException,
                    "{IpAddress}:{Port} - {EventName}",
                    IpAddress, Port, LoggingEvents.ServerStopException.Name);
        }
    }

    /// <summary>
    /// Closes the connection to the server.
    /// </summary>
    /// <param name="connectionId">The ID of the connection to close.</param>
    /// <returns>true if the connection was closed; otherwise - false.</returns>
    public bool CloseConnection(Guid connectionId)
    {
        try
        {
            _connectedClients[connectionId].Close();

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
            _ = _server!.BeginAcceptTcpClient(HandleConnection, _server);
        }
        catch (Exception ex)
        {
            _requestHandlingException = ex;

            RequestHandlingFailBeforeConnection?.Invoke(this, _requestHandlingException);

            _logger?.LogCritical(LoggingEvents.RequestHandlingFailBeforeConnection,
                _requestHandlingException,
                "{EventName}",
                LoggingEvents.RequestHandlingFailBeforeConnection.Name);

            Stop();
        }
    }

    private void HandleConnection(IAsyncResult result)
    {
        if (_isServerStarted is false)
            return;

        AcceptConnection();

        TcpClient client = _server!.EndAcceptTcpClient(result);

        TcpConnection connection = new(Guid.NewGuid(), (IPEndPoint)client.Client.RemoteEndPoint!);
        _ = _connectedClients.TryAdd(connection.Id, client);

        ConnectionOpen?.Invoke(this, connection);

        _logger?.LogInformation(LoggingEvents.ConnectionOpen,
            "{ConnectionId} - {EventName}",
            connection.Id, LoggingEvents.ConnectionOpen.Name);

        try
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = _requestReadTimeout;

            HandleRequest(connection, stream);
        }
        catch (Exception ex)
        {
            _requestHandlingException = ex;

            RequestHandlingFailAfterConnection?.Invoke(this, (connection, _requestHandlingException));

            _logger?.LogError(LoggingEvents.RequestHandlingFailAfterConnection,
                _requestHandlingException,
                "{ConnectionId} - {EventName}",
                connection.Id, LoggingEvents.RequestHandlingFailAfterConnection.Name);
        }
        finally
        {
            client.Close();
            _ = _connectedClients.TryRemove(connection.Id, out _);

            ConnectionClose?.Invoke(this, connection);

            _logger?.LogInformation(LoggingEvents.ConnectionClose,
                "{ConnectionId} - {EventName}",
                connection.Id, LoggingEvents.ConnectionClose.Name);
        }
    }

    private void HandleRequest(TcpConnection connection, NetworkStream stream)
    {
        IEnumerable<byte> requestBody = Enumerable.Empty<byte>();

        do
        {
            byte[] requestBuffer = new byte[_requestBufferSize];
            int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);

            requestBody = requestBody.Concat(requestBuffer.Take(bytesRead));
        }
        while (stream.DataAvailable);

        TcpRequest request = new(requestBody, connection);
        TcpResponse? response = RequestHandler?.Invoke(request);

        byte[] responseBytes = (response?.Body?.Any() is true) ? response.Body.ToArray() : new byte[] { default };

        stream.Write(responseBytes, 0, responseBytes.Length);

        RequestHandling?.Invoke(this, request);

        _logger?.LogInformation(LoggingEvents.RequestHandling,
            "{ConnectionId} - {EventName}. Bytes received: {RequestSize}, bytes sent: {ResponseSize}",
            connection.Id, LoggingEvents.RequestHandling.Name, requestBody.Count(), responseBytes.Length);

        if (response?.KeepConnectionAlive is true)
            HandleRequest(connection, stream);
    }
}