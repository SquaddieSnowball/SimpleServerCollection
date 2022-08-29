using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Text;

namespace SimpleHttpServer.Modules.Helpers;

/// <summary>
/// Provides methods used to build HTTP responses.
/// </summary>
internal sealed class HttpResponseBuilder
{
    /// <summary>
    /// Creates an HTTP response from an instance of the HttpResponse class.
    /// </summary>
    /// <param name="response">An instance of the HttpResponse class.</param>
    /// <returns>Sequence of bytes containing the HTTP response.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static IEnumerable<byte> Build(HttpResponse response)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response must not be null.");

        var responseStringBuilder = new StringBuilder();

        string protocolVersion = response.ProtocolVersion;
        string statusCode = ((int)response.Status).ToString();
        string statusMessage = response.Status.ToString();

        responseStringBuilder.Append("HTTP/" + protocolVersion + ' ' + statusCode + ' ' + statusMessage);

        if (response.Headers is not null)
            foreach (HttpHeader header in response.Headers)
                responseStringBuilder.Append('\n' + header.Parameter + ": " + header.Value);

        if (string.IsNullOrEmpty(response.Body) is false)
            responseStringBuilder.Append("\n\n" + response.Body);

        byte[] responseBytes = Encoding.ASCII.GetBytes(responseStringBuilder.ToString());

        return responseBytes;
    }
}