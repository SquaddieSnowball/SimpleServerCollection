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
    private readonly ILogger<TcpServer>? _logger;
    private bool _isServerStarted;
    private int _requestBufferSize = 1024;
    private int _requestReadTimeout = Timeout.Infinite;
    private readonly ConcurrentDictionary<Guid, TcpClient> _connectedClients = new();
    private Exception? _stopException;
    private Exception? _requestHandlingException;

    /// <summary>
    /// Gets an IPAddress that represents the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Gets the port on which to listen for requests.
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
    /// Gets or sets the amount of time waiting for the request data.
    /// </summary>
    public int RequestReadTimeout
    {
        get => _requestReadTimeout;
        set
        {
            if (value is not Timeout.Infinite and < 1)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "The request read timeout value must be \"Timeout.Infinite\" or greater than 0.");

            _requestReadTimeout = value;
        }
    }

    /// <summary>
    /// The method used to handle a request.
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
    /// Occurs when the connection is opened.
    /// </summary>
    public event EventHandler<TcpConnection>? ConnectionOpen;

    /// <summary>
    /// Occurs when the connection is closed.
    /// </summary>
    public event EventHandler<TcpConnection>? ConnectionClose;

    /// <summary>
    /// Occurs when request handling finishes.
    /// </summary>
    public event EventHandler<TcpRequest>? RequestHandling;

    /// <summary>
    /// Occurs when request handling fails before connection.
    /// </summary>
    public event EventHandler<Exception>? RequestHandlingFailBeforeConnection;

    /// <summary>
    /// Occurs when request handling fails after connection.
    /// </summary>
    public event EventHandler<(Exception, TcpConnection)>? RequestHandlingFailAfterConnection;

    /// <summary>
    /// Initializes a new instance of the TcpServer class that runs a TCP server
    /// which listens for requests on the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">An IPAddress that represents the local IP address.</param>
    /// <param name="port">The port on which to listen for requests.</param>
    public TcpServer(IPAddress ipAddress, int port) =>
        (IpAddress, Port) = (ipAddress, port);

    /// <summary>
    /// Initializes a new instance of the TcpServer class that runs a TCP server
    /// which listens for requests on the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">An IPAddress that represents the local IP address.</param>
    /// <param name="port">The port on which to listen for requests.</param>
    /// <param name="logger">The logger that will be used to log events.</param>
    public TcpServer(IPAddress ipAddress, int port, ILogger<TcpServer> logger) : this(ipAddress, port) =>
        _logger = logger;

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

            _logger?.LogInformation(LoggingEvents.ServerStart, "{EventName} at {IpAddress}:{Port}",
                LoggingEvents.ServerStart.Name, IpAddress, Port);
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
                _logger?.LogInformation(LoggingEvents.ServerStop, "{EventName} at {IpAddress}:{Port}",
                    LoggingEvents.ServerStop.Name, IpAddress, Port);
            else
                _logger?.LogCritical(LoggingEvents.ServerStopException, _stopException, "{EventName} at {IpAddress}:{Port}",
                    LoggingEvents.ServerStopException.Name, IpAddress, Port);
        }
    }

    /// <summary>
    /// Closes the connection to the server.
    /// </summary>
    /// <param name="connectionId">ID of the connection to close.</param>
    /// <returns>true if the connection was closed; false if the connection is already closed.</returns>
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
    }

    private void AcceptConnection()
    {
        try
        {
            _server!.BeginAcceptTcpClient(HandleConnection, _server);
        }
        catch (Exception ex)
        {
            _requestHandlingException = ex;

            RequestHandlingFailBeforeConnection?.Invoke(this, _requestHandlingException);

            _logger?.LogCritical(LoggingEvents.RequestHandlingFailBeforeConnection, _requestHandlingException,
                "{EventName}", LoggingEvents.RequestHandlingFailBeforeConnection.Name);

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

        _logger?.LogInformation(LoggingEvents.ConnectionOpen, "{EventName} on {IpAddress}:{Port} (ID: {ConnectionId})",
            LoggingEvents.ConnectionOpen.Name, connection.RemoteEndPoint.Address,
            connection.RemoteEndPoint.Port, connection.Id);

        try
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = RequestReadTimeout;

            HandleRequest(connection, stream);
        }
        catch (Exception ex)
        {
            _requestHandlingException = ex;

            RequestHandlingFailAfterConnection?.Invoke(this, (_requestHandlingException, connection));

            _logger?.LogError(LoggingEvents.RequestHandlingFailAfterConnection, _requestHandlingException,
                "{EventName} (Conndection ID: {ConnectionId})",
                LoggingEvents.RequestHandlingFailAfterConnection.Name, connection.Id);
        }
        finally
        {
            client.Close();
            _ = _connectedClients.TryRemove(connection.Id, out _);

            ConnectionClose?.Invoke(this, connection);

            _logger?.LogInformation(LoggingEvents.ConnectionClose, "{EventName} (ID: {ConnectionId})",
                LoggingEvents.ConnectionClose.Name, connection.Id);
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
        while (stream.DataAvailable);

        TcpRequest request = new(requestBody, connection);
        TcpResponse? response = RequestHandler?.Invoke(request);

        byte[] responseBytes =
            (response is not null) && (response.Body is not null) && (response.Body.Any() is true) ?
            response.Body.ToArray() :
            new byte[] { default };

        stream.Write(responseBytes, 0, responseBytes.Length);

        RequestHandling?.Invoke(this, request);

        _logger?.LogInformation(LoggingEvents.RequestHandling, "{EventName} (Conndection ID: {ConnectionId})",
            LoggingEvents.RequestHandling.Name, connection.Id);

        if (response?.KeepConnectionAlive is true)
            HandleRequest(connection, stream);
    }
}