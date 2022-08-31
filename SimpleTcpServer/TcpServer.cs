using System.Net;
using System.Net.Sockets;

namespace SimpleTcpServer;

/// <summary>
/// Runs a server that listens for requests from TCP network clients.
/// </summary>
public sealed class TcpServer
{
    private TcpListener? _server;
    private int _requestBufferSize = 1024;
    private int _requestReadTimeout = Timeout.Infinite;
    private bool _serverStarted;
    private Exception? _requestHandlingException;
    private Exception? _stopException;

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
                throw new ArgumentOutOfRangeException(nameof(value), "The request buffer size value must be greater than 0.");

            _requestBufferSize = value;
        }
    }

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
    public Func<IEnumerable<byte>, IEnumerable<byte>>? RequestHandler { get; set; }

    /// <summary>
    /// Occurs when the server starts.
    /// </summary>
    public event EventHandler? ServerStart;

    public event EventHandler<Exception>? RequestHandlingFail;

    /// <summary>
    /// Occurs when the server is stopped.
    /// </summary>
    public event EventHandler<Exception?>? ServerStop;

    /// <summary>
    /// Initializes a new instance of the TcpServer class that runs a TCP server
    /// which listens for requests on the specified local IP address and port number.
    /// </summary>
    /// <param name="ipAddress">An IPAddress that represents the local IP address.</param>
    /// <param name="port">The port on which to listen for requests.</param>
    public TcpServer(IPAddress ipAddress, int port) => (IpAddress, Port) = (ipAddress, port);

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        if (_serverStarted is true)
            return;

        try
        {
            _server = new TcpListener(IpAddress, Port);
            _server.Start();

            _serverStarted = true;
            ServerStart?.Invoke(this, EventArgs.Empty);
        }
        catch (ArgumentNullException)
        {
            throw;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw;
        }
        catch (SocketException)
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
            _serverStarted = false;
            ServerStop?.Invoke(this, _stopException);
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
        if (_serverStarted is false)
            return;

        AcceptConnection();

        using TcpClient client = _server!.EndAcceptTcpClient(result);

        try
        {
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = RequestReadTimeout;

            IEnumerable<byte> request = Enumerable.Empty<byte>();

            do
            {
                var requestBuffer = new byte[RequestBufferSize];
                int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
                request = request.Concat(requestBuffer.Take(bytesRead));
            }
            while (stream.DataAvailable);

            IEnumerable<byte> response = Enumerable.Empty<byte>();

            if (RequestHandler is not null)
                response = RequestHandler(request);

            byte[] responseBytes =
                (response is not null) && response.Any() ?
                response.ToArray() :
                new byte[] { default };

            stream.Write(responseBytes, 0, responseBytes.Length);
        }
        catch (Exception ex)
        {
            _requestHandlingException = ex;
            RequestHandlingFail?.Invoke(this, _requestHandlingException);
        }
    }
}