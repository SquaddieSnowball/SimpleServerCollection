using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;

namespace SimpleHttpServer.Services.Abstractions;

/// <summary>
/// Provides functionality for generating HTTP server responses.
/// </summary>
public interface IHttpServerResponseGenerator
{
    /// <summary>
    /// Gets or sets an instance of the <see cref="HttpServer"/> class for which responses will be generated.
    /// </summary>
    public HttpServer? Server { get; set; }

    /// <summary>
    /// Generates an HTTP response for the specified status.
    /// </summary>
    /// <param name="status">Response status.</param>
    /// <param name="requiredHeaders">HTTP headers required to generate a response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    public HttpResponse GenerateResponse(HttpResponseStatus status, IEnumerable<HttpHeader>? requiredHeaders = default);
}