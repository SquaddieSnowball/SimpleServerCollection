using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using SimpleHttpServer.Modules.Helpers;
using System.Text;

namespace SimpleHttpServer.Tests.Helpers;

[TestClass]
public sealed class HttpResponseBuilderTests
{
    [TestMethod]
    public void Build_SingleLineResponse_ReturnsCorrectBytes()
    {
        string textResponse = "HTTP/1.0 200 OK";
        IEnumerable<byte> expected = Encoding.ASCII.GetBytes(textResponse);
        HttpResponse httpResponse = new("1.0", HttpResponseStatus.OK, Enumerable.Empty<HttpHeader>());

        IEnumerable<byte> actual = HttpResponseBuilder.Build(httpResponse);

        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }

    [TestMethod]
    public void Build_ResponseWithHeader_ReturnsCorrectBytes()
    {
        string textResponse = "HTTP/1.0 200 OK\nHeader: value";
        IEnumerable<byte> expected = Encoding.ASCII.GetBytes(textResponse);
        HttpResponse httpResponse = new("1.0", HttpResponseStatus.OK, new HttpHeader[]
            { new HttpHeader(HttpHeaderGroup.Response, "Header", "value") });

        IEnumerable<byte> actual = HttpResponseBuilder.Build(httpResponse);

        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }

    [TestMethod]
    public void Build_ResponseWithHeaderAndBody_ReturnsCorrectBytes()
    {
        string textResponse = "HTTP/1.0 200 OK\nHeader: value\n\nBody";
        IEnumerable<byte> expected = Encoding.ASCII.GetBytes(textResponse);
        HttpResponse httpResponse = new("1.0", HttpResponseStatus.OK, new HttpHeader[]
            { new HttpHeader(HttpHeaderGroup.Response, "Header", "value") }, "Body");

        IEnumerable<byte> actual = HttpResponseBuilder.Build(httpResponse);

        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }
}