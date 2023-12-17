using SimpleHttpServer.Entities;

namespace SimpleHttpServer.Services.Abstractions;

/// <summary>
/// Provides a method used to build HTTP responses.
/// </summary>
public interface IHttpResponseBuilder
{
    /// <summary>
    /// Creates a byte sequence containing an HTTP response from instances of the 
    /// <see cref="HttpRequest"/> and <see cref="HttpResponse"/> class.
    /// </summary>
    /// <param name="request">An instance of the <see cref="HttpRequest"/> class.</param>
    /// <param name="response">An instance of the <see cref="HttpResponse"/> class.</param>
    /// <returns>A sequence of bytes containing the HTTP response.</returns>
    public IEnumerable<byte> Build(HttpRequest? request, HttpResponse response);
}