using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Text;

namespace SimpleHttpServer.Modules.Helpers;

internal sealed class HttpResponseBuilder
{
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