using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;
using SimpleHttpServer.Services.Implementations;
using System.Text;

namespace SimpleHttpServer.UnitTests.Services;

[TestClass]
public sealed class HttpResponseBuilderTests
{
    [TestMethod]
    public void Build_SingleLineResponse_ReturnsCorrectBytes()
    {
        HttpResponseBuilder builder = CreateHttpResponseBuilder();
        HttpRequest httpRequest = new(
            Enumerable.Empty<byte>(),
            HttpRequestMethod.GET,
            "/",
            Enumerable.Empty<HttpRequestQueryParameter>(),
            "1.0",
            Enumerable.Empty<HttpHeader>());
        HttpResponse httpResponse = new(
            HttpResponseStatus.OK,
            Enumerable.Empty<HttpHeader>());
        string responseString = "HTTP/1.0 200 OK";
        IEnumerable<byte> expected = Encoding.UTF8.GetBytes(responseString);

        IEnumerable<byte> actual = builder.Build(httpRequest, httpResponse);

        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }

    [TestMethod]
    public void Build_ResponseWithHeader_ReturnsCorrectBytes()
    {
        HttpResponseBuilder builder = CreateHttpResponseBuilder();
        HttpRequest httpRequest = new(
            Enumerable.Empty<byte>(),
            HttpRequestMethod.GET,
            "/",
            Enumerable.Empty<HttpRequestQueryParameter>(),
            "1.0",
            Enumerable.Empty<HttpHeader>());
        HttpResponse httpResponse = new(
            HttpResponseStatus.OK,
            new HttpHeader[]
            {
                new(HttpHeaderGroup.Response, "Header", "value")
            });
        string responseString = "HTTP/1.0 200 OK\r\nHeader: value";
        IEnumerable<byte> expected = Encoding.UTF8.GetBytes(responseString);

        IEnumerable<byte> actual = builder.Build(httpRequest, httpResponse);

        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }

    [TestMethod]
    public void Build_ResponseWithHeaderAndBody_ReturnsCorrectBytes()
    {
        HttpResponseBuilder builder = CreateHttpResponseBuilder();
        HttpRequest httpRequest = new(
            Enumerable.Empty<byte>(),
            HttpRequestMethod.GET,
            "/",
            Enumerable.Empty<HttpRequestQueryParameter>(),
            "1.0",
            Enumerable.Empty<HttpHeader>());
        HttpResponse httpResponse = new(
            HttpResponseStatus.OK,
            new HttpHeader[]
            {
                new(HttpHeaderGroup.Response, "Header", "value")
            },
            Encoding.UTF8.GetBytes("Body"));
        string responseString = "HTTP/1.0 200 OK\r\nHeader: value\r\n\r\nBody";
        IEnumerable<byte> expected = Encoding.UTF8.GetBytes(responseString);

        IEnumerable<byte> actual = builder.Build(httpRequest, httpResponse);

        CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
    }

    private static HttpResponseBuilder CreateHttpResponseBuilder() => new();
}