using SimpleHttpServer.Modules.Helpers;
using System.Text;

namespace SimpleHttpServer.Tests.Helpers;

[TestClass]
public sealed class HttpRequestParserTests
{
    [TestMethod]
    public void ParseFromBytes_EmptyString_ThrowsArgumentException()
    {
        string request = string.Empty;
        IEnumerable<byte> requestBytes = Encoding.ASCII.GetBytes(request);

        void actual() => _ = HttpRequestParser.ParseFromBytes(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    public void ParseFromBytes_InvalidStartLine_ThrowsArgumentException()
    {
        string request = "GET";
        IEnumerable<byte> requestBytes = Encoding.ASCII.GetBytes(request);

        void actual() => _ = HttpRequestParser.ParseFromBytes(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    public void ParseFromBytes_IvalidTargetParameter_ThrowsArgumentException()
    {
        string request = "GET /?parameter HTTP/1.0";
        IEnumerable<byte> requestBytes = Encoding.ASCII.GetBytes(request);

        void actual() => _ = HttpRequestParser.ParseFromBytes(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    public void ParseFromBytes_InvalidHeader_ThrowsArgumentException()
    {
        string request = "GET / HTTP/1.0\nHeader";
        IEnumerable<byte> requestBytes = Encoding.ASCII.GetBytes(request);

        void actual() => _ = HttpRequestParser.ParseFromBytes(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    [DataRow("GET / HTTP/1.0")]
    [DataRow("GET /?parameter=value HTTP/1.0")]
    [DataRow("GET / HTTP/1.0\nHeader: value")]
    [DataRow("GET / HTTP/1.0\n\nBody")]
    [DataRow("GET /?parameter=value HTTP/1.0\nHeader: value\n\nBody")]
    public void ParseFromBytes_MultipleRequests_DoesNotThrowAnException(string request)
    {
        IEnumerable<byte> requestBytes = Encoding.ASCII.GetBytes(request);

        _ = HttpRequestParser.ParseFromBytes(requestBytes);

        Assert.IsTrue(true);
    }
}