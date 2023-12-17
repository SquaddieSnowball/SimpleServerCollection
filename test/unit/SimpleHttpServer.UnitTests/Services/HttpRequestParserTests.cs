using SimpleHttpServer.Services.Implementations;
using System.Text;

namespace SimpleHttpServer.UnitTests.Services;

[TestClass]
public sealed class HttpRequestParserTests
{
    [TestMethod]
    public void Parse_InvalidStartLine_ThrowsArgumentException()
    {
        HttpRequestParser parser = CreateHttpRequestParser();
        string requestString = "GET";
        IEnumerable<byte> requestBytes = Encoding.UTF8.GetBytes(requestString);

        void actual() => _ = parser.Parse(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    public void Parse_IvalidQueryParameter_ThrowsArgumentException()
    {
        HttpRequestParser parser = CreateHttpRequestParser();
        string requestString = "GET /?parameter HTTP/1.0";
        IEnumerable<byte> requestBytes = Encoding.UTF8.GetBytes(requestString);

        void actual() => _ = parser.Parse(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    public void Parse_InvalidHeader_ThrowsArgumentException()
    {
        HttpRequestParser parser = CreateHttpRequestParser();
        string requestString = "GET / HTTP/1.0\r\nHeader";
        IEnumerable<byte> requestBytes = Encoding.UTF8.GetBytes(requestString);

        void actual() => _ = parser.Parse(requestBytes);

        _ = Assert.ThrowsException<ArgumentException>(actual);
    }

    [TestMethod]
    [DataRow("GET / HTTP/1.0")]
    [DataRow("GET /?parameter=value HTTP/1.0")]
    [DataRow("GET / HTTP/1.0\r\nHeader: value")]
    [DataRow("GET /?parameter=value HTTP/1.0\r\nHeader: value")]
    [DataRow("GET / HTTP/1.0\r\n\r\nBody")]
    [DataRow("GET /?parameter=value HTTP/1.0\r\n\r\nBody")]
    [DataRow("GET / HTTP/1.0\r\nHeader: value\r\n\r\nBody")]
    [DataRow("GET /?parameter=value HTTP/1.0\r\nHeader: value\r\n\r\nBody")]
    public void Parse_ValidRequest_DoesNotThrowAnException(string requestString)
    {
        HttpRequestParser parser = CreateHttpRequestParser();
        IEnumerable<byte> requestBytes = Encoding.UTF8.GetBytes(requestString);

        _ = parser.Parse(requestBytes);

        Assert.IsTrue(true);
    }

    private static HttpRequestParser CreateHttpRequestParser() => new();
}