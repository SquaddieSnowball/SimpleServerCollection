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
    private bool _serverStarted;
    private Exception? _stopException;

    /// <summary>
    /// An IPAddress that represents the local IP address.
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// The port on which to listen for requests.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// The size of the byte array used as a buffer to store the request.
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

    /// <summary>
    /// The method used to handle a request.
    /// </summary>
    public Func<IEnumerable<byte>, IEnumerable<byte>>? RequestHandler { get; set; }

    /// <summary>
    /// Occurs when the server starts.
    /// </summary>
    public event EventHandler? ServerStart;

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

    private void AcceptConnection() => _server!.BeginAcceptTcpClient(HandleConnection, _server);

    private void HandleConnection(IAsyncResult result)
    {
        if (_serverStarted is false)
            return;

        AcceptConnection();

        using TcpClient client = _server!.EndAcceptTcpClient(result);

        NetworkStream stream = client.GetStream();

        IEnumerable<byte> request = Enumerable.Empty<byte>();

        while (stream.DataAvailable)
        {
            var requestBuffer = new byte[RequestBufferSize];
            int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
            request = request.Concat(requestBuffer.Take(bytesRead));
        }

        IEnumerable<byte> response = Enumerable.Empty<byte>();

        if (RequestHandler is not null)
            response = RequestHandler(request);

        byte[] responseBytes =
            (response is not null) && response.Any() ?
            response.ToArray() :
            new byte[] { default };

        stream.Write(responseBytes, 0, responseBytes.Length);
    }
}