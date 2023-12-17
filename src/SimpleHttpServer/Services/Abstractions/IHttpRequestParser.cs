using SimpleHttpServer.Entities;

namespace SimpleHttpServer.Services.Abstractions;

/// <summary>
/// Provides a method used to parse HTTP requests.
/// </summary>
public interface IHttpRequestParser
{
    /// <summary>
    /// Parses an HTTP request from a sequence of bytes.
    /// </summary>
    /// <param name="request">The sequence of bytes containing the HTTP request.</param>
    /// <returns>A new instance of the <see cref="HttpRequest"/> class.</returns>
    public HttpRequest Parse(IEnumerable<byte> request);
}