using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Text;

namespace SimpleHttpServer.Modules.Helpers;

/// <summary>
/// Provides methods used to build HTTP responses.
/// </summary>
internal sealed class HttpResponseBuilder
{
    private readonly Dictionary<string, string> _statusCodeMessageCollection = new();

    public HttpResponseBuilder()
    {
        try
        {
            string[] statusStrings =
                Properties.Resources.HttpResponseStatusMessages.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (string statusString in statusStrings)
            {
                string statusCode = string.Concat(statusString.TakeWhile(c => char.IsDigit(c) is true));
                string statusMessage = string.Concat(statusString.SkipWhile(c => char.IsDigit(c) is true).Skip(1));

                _statusCodeMessageCollection.Add(statusCode, statusMessage);
            }
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Creates an HTTP response from an instance of the HttpResponse class.
    /// </summary>
    /// <param name="response">An instance of the HttpResponse class.</param>
    /// <returns>Sequence of bytes containing the HTTP response.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal IEnumerable<byte> Build(HttpResponse response)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response), "Response must not be null.");

        var responseStringBuilder = new StringBuilder();

        string protocolVersion = response.ProtocolVersion;
        string statusCode = ((int)response.Status).ToString();
        string statusMessage =
            _statusCodeMessageCollection.ContainsKey(statusCode) is true ?
            _statusCodeMessageCollection[statusCode] :
            response.Status.ToString();

        responseStringBuilder.Append("HTTP/" + protocolVersion + ' ' + statusCode + ' ' + statusMessage);

        if (response.Headers is not null)
            foreach (HttpHeader header in response.Headers.OrderBy(h => h.Group))
                responseStringBuilder.Append('\n' + header.Parameter + ": " + header.Value);

        if (string.IsNullOrEmpty(response.Body) is false)
            responseStringBuilder.Append("\n\n" + response.Body);

        byte[] responseBytes = Encoding.ASCII.GetBytes(responseStringBuilder.ToString());

        return responseBytes;
    }
}