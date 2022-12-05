using SimpleTcpServer.Modules.Entities;
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
    private readonly Dictionary<Guid, TcpClient> _connectedClients = new();
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
    /// Occurs when request handling fails.
    /// </summary>
    public event EventHandler<Exception>? RequestHandlingFail;

    /// <summary>
    /// Initializes a new instance of the TcpServer class that runs a TCP server
    /// which listens for requests on the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">An IPAddress that represents the local IP address.</param>
    /// <param name="port">The port on which to listen for requests.</param>
    public TcpServer(IPAddress ipAddress, int port) =>
        (IpAddress, Port) = (ipAddress, port);

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
        }
    }

    /// <summary>
    /// Closes the connection to the server.
    /// </summary>
    /// <param name="connectionId">Connection ID.</param>
    /// <exception cref="ArgumentException"></exception>
    public void CloseConnection(Guid connectionId)
    {
        try
        {
            _connectedClients[connectionId].Close();
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException("The client with the specified ID does not exist.");
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
            RequestHandlingFail?.Invoke(this, _requestHandlingException);

            Stop();
        }
    }

    private void HandleConnection(IAsyncResult result)
    {
        if (_isServerStarted is false)
            return;

        AcceptConnection();

        Guid connectionId = Guid.NewGuid();
        TcpClient client = _server!.EndAcceptTcpClient(result);

        _connectedClients.Add(connectionId, client);

        try
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = RequestReadTimeout;

            HandleRequest(connectionId, stream);
        }
        catch (Exception ex)
        {
            _requestHandlingException = ex;
            RequestHandlingFail?.Invoke(this, _requestHandlingException);
        }
        finally
        {
            client.Close();
            _ = _connectedClients.Remove(connectionId);
        }
    }

    private void HandleRequest(Guid connectionId, NetworkStream stream)
    {
        IEnumerable<byte> requestBody = Enumerable.Empty<byte>();

        do
        {
            byte[] requestBuffer = new byte[RequestBufferSize];
            int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
            requestBody = requestBody.Concat(requestBuffer.Take(bytesRead));
        }
        while (stream.DataAvailable);

        TcpRequest? request = new(connectionId, requestBody);
        TcpResponse? response = default;

        if (RequestHandler is not null)
            response = RequestHandler(request);

        if (response is not null)
        {
            byte[] responseBytes =
                (response.Body is not null) && (response.Body.Any() is true) ?
                response.Body.ToArray() :
                new byte[] { default };

            stream.Write(responseBytes, 0, responseBytes.Length);

            if (response.KeepConnectionAlive is true)
                HandleRequest(connectionId, stream);
        }
        else
            stream.Write(new byte[] { default }, 0, 1);
    }
}