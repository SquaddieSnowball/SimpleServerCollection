using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;
using SimpleHttpServer.Resources.Entities.HttpResponse;
using SimpleHttpServer.Services.Abstractions;
using System.Resources;
using System.Text;
using Validation.Helpers;

namespace SimpleHttpServer.Services.Implementations;

/// <summary>
/// Provides a method used to build HTTP responses.
/// </summary>
public sealed class HttpResponseBuilder : IHttpResponseBuilder
{
    private readonly ResourceManager _responseStatusMessagesResourceManager = new(typeof(HttpResponseStatusMessages));

    /// <summary>
    /// Creates a byte sequence containing an HTTP response from instances of the 
    /// <see cref="HttpRequest"/> and <see cref="HttpResponse"/> class.
    /// </summary>
    /// <param name="request">An instance of the <see cref="HttpRequest"/> class.</param>
    /// <param name="response">An instance of the <see cref="HttpResponse"/> class.</param>
    /// <returns>A sequence of bytes containing the HTTP response.</returns>
    public IEnumerable<byte> Build(HttpRequest? request, HttpResponse response)
    {
        Verify.NotNull(response);
        Verify.EnumerationValue(response.Status);

        StringBuilder responseStringBuilder = new();

        string protocolVersion =
            ((request is null) || (HttpServer.SupportedProtocolVersions.Contains(request.ProtocolVersion) is false)) ?
            HttpServer.SupportedProtocolVersions.Last() :
            request.ProtocolVersion;

        string statusCode = ((int)response.Status).ToString();
        string statusMessage = _responseStatusMessagesResourceManager.GetString(statusCode) ?? response.Status.ToString();

        _ = responseStringBuilder.Append($"HTTP/{protocolVersion} {statusCode} {statusMessage}");

        if (response.Headers?.Any() is true)
        {
            foreach (HttpHeader header in response.Headers.OrderBy(h => h.Group))
                _ = responseStringBuilder.Append($"\r\n{header.Parameter}: {header.Value}");
        }

        IEnumerable<byte> responseBytes = Encoding.UTF8.GetBytes(responseStringBuilder.ToString());

        if (response.Body?.Any() is true)
            responseBytes = responseBytes.Concat(Encoding.UTF8.GetBytes("\r\n\r\n")).Concat(response.Body);

        return responseBytes;
    }
}