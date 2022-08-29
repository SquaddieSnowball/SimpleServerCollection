using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.Modules.Helpers;

internal sealed class HttpRequestParser
{
    private const string StartLinePattern = @"^(?<method>\w+)\s(?<target>.+)\sHTTP/(?<protocolVersion>\d(.\d)*)$";
    private const string QueryParameterPattern = @"^(?<parameter>.+)=(?<value>.+)$";
    private const string HeaderPattern = @"^(?<parameter>.+):\s(?<value>.+)$";

    internal static HttpRequest ParseFromBytes(IEnumerable<byte> request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "Request must not be null.");

        string requestString = Encoding.ASCII.GetString(request.ToArray());

        using var stringReader = new StringReader(requestString);

        string? currentLine = stringReader.ReadLine();

        if (currentLine is null)
            throw new ArgumentException("Parse error: start line not found.", nameof(request));

        Match startLineMatch = Regex.Match(currentLine, StartLinePattern);

        if (startLineMatch.Success is false)
            throw new ArgumentException("Parse error: start line does not match pattern.", nameof(request));

        var headerMatches = new List<Match>();
        string body = string.Empty;

        while ((currentLine = stringReader.ReadLine()) is not null)
        {
            if (currentLine.Equals(string.Empty, StringComparison.Ordinal))
            {
                body = stringReader.ReadToEnd().Trim();

                break;
            }

            Match headerMatch = Regex.Match(currentLine, HeaderPattern);

            if (headerMatch.Success is false)
                throw new ArgumentException("Parse error: one or more headers do not match the pattern.", nameof(request));

            headerMatches.Add(headerMatch);
        }

        if (Enum.TryParse(startLineMatch.Groups["method"].Value, out HttpRequestMethod method) is false)
            method = HttpRequestMethod.NOTIMPLEMENTED;

        string target;
        var queryParameters = new List<HttpRequestQueryParameter>();

        if (startLineMatch.Groups["target"].Value.Contains('?'))
        {
            target = string.Concat(startLineMatch.Groups["target"].Value.TakeWhile(c => c.Equals('?') is false));

            string[] queryParameterStrings =
                string.Concat(startLineMatch.Groups["target"].Value.SkipWhile(c => c.Equals('?') is false).Skip(1))
                .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string queryParameterString in queryParameterStrings)
            {
                Match queryParameterMatch = Regex.Match(queryParameterString, QueryParameterPattern);

                if (queryParameterMatch.Success is false)
                    throw new ArgumentException("Parse error: one or more query parameters do not match the pattern.", nameof(request));

                queryParameters.Add(new HttpRequestQueryParameter(
                    queryParameterMatch.Groups["parameter"].Value,
                    queryParameterMatch.Groups["value"].Value));
            }
        }
        else
            target = startLineMatch.Groups["target"].Value;

        string protocolVersion = startLineMatch.Groups["protocolVersion"].Value;

        var headers = new List<HttpHeader>();

        foreach (Match headerMatch in headerMatches)
            headers.Add(new HttpHeader(
                HttpHeaderGroup.Unknown,
                headerMatch.Groups["parameter"].Value,
                headerMatch.Groups["value"].Value));

        var httpRequest = new HttpRequest(method, target, queryParameters, protocolVersion, headers, body);

        return httpRequest;
    }
}