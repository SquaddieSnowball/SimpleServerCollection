using System.Net;
using System.Net.Sockets;

namespace SimpleTcpServer;

public sealed class TcpServer
{
    private TcpListener? _server;
    private int _requestBufferSize = 1024;
    private bool _serverStarted;
    private Exception? _stopException;

    public IPAddress IpAddress { get; }

    public int Port { get; }

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

    public Func<IEnumerable<byte>, IEnumerable<byte>>? RequestHandler { get; set; }

    public event EventHandler? ServerStart;

    public event EventHandler<Exception?>? ServerStop;

    public TcpServer(IPAddress ipAddress, int port) => (IpAddress, Port) = (ipAddress, port);

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