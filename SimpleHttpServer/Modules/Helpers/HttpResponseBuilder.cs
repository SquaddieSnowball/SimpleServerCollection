using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Text;

namespace SimpleHttpServer.Modules.Helpers;

/// <summary>
/// Provides methods used to build HTTP responses.
/// </summary>
internal static class HttpResponseBuilder
{
    private static readonly Dictionary<string, string> s_statusCodeMessageCollection = new();

    static HttpResponseBuilder()
    {
        try
        {
            string[] statusCodeMessageStrings =
                Properties.Resources.HttpResponseStatusMessages.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string statusCodeMessageString in statusCodeMessageStrings)
            {
                string statusCode = string.Concat(statusCodeMessageString.TakeWhile(c => char.IsDigit(c) is true));
                string statusMessage = string.Concat(statusCodeMessageString.SkipWhile(c => char.IsDigit(c) is true).Skip(1));

                s_statusCodeMessageCollection.Add(statusCode, statusMessage);
            }
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Creates a byte-based HTTP response from an instance of the HttpResponse class.
    /// </summary>
    /// <param name="response">An instance of the HttpResponse class.</param>
    /// <returns>A sequence of bytes containing the HTTP response.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IEnumerable<byte> Build(HttpResponse response)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response), "The response must not be null.");

        StringBuilder responseStringBuilder = new();

        string protocolVersion = response.ProtocolVersion;
        string statusCode = ((int)response.Status).ToString();
        string statusMessage = s_statusCodeMessageCollection.GetValueOrDefault(statusCode) ?? response.Status.ToString();

        _ = responseStringBuilder.Append($"HTTP/{protocolVersion} {statusCode} {statusMessage}");

        if (response.Headers is not null)
        {
            foreach (HttpHeader header in response.Headers.OrderBy(h => h.Group))
                _ = responseStringBuilder.Append($"\n{header.Parameter}: {header.Value}");
        }

        if (string.IsNullOrEmpty(response.Body) is false)
            _ = responseStringBuilder.Append($"\n\n{response.Body}");

        byte[] responseBytes = Encoding.ASCII.GetBytes(responseStringBuilder.ToString());

        return responseBytes;
    }
}