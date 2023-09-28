using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.Modules.Helpers;

/// <summary>
/// Provides methods used to parse HTTP requests.
/// </summary>
internal static class HttpRequestParser
{
    private const string StartLinePattern = @"^(?<method>\w+)\s(?<target>.+)\sHTTP/(?<protocolVersion>\d(.\d)*)$";
    private const string QueryParameterPattern = @"^(?<parameter>.+)=(?<value>.+)$";
    private const string HeaderPattern = @"^(?<parameter>.+):\s(?<value>.+)$";

    /// <summary>
    /// Parses an HTTP request from a sequence of bytes.
    /// </summary>
    /// <param name="request">The sequence of bytes containing the HTTP request.</param>
    /// <returns>A new instance of the HttpRequest class.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static HttpRequest ParseFromBytes(IEnumerable<byte> request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request), "The request must not be null.");

        string requestString = Encoding.ASCII.GetString(request.ToArray());
        using StringReader requestStringReader = new(requestString);
        string? currentLine;

        currentLine = requestStringReader.ReadLine() ??
            throw new ArgumentException("Parse error: start line not found.", nameof(request));

        Match startLineMatch = Regex.Match(currentLine, StartLinePattern);

        if (startLineMatch.Success is false)
            throw new ArgumentException("Parse error: start line does not match pattern.", nameof(request));

        List<Match> headerMatches = new();
        string? body = default;

        while ((currentLine = requestStringReader.ReadLine()) is not null)
        {
            if (currentLine.Equals(string.Empty, StringComparison.Ordinal) is true)
            {
                body = requestStringReader.ReadToEnd().Trim();

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
        List<HttpRequestQueryParameter> queryParameters = new();

        if (startLineMatch.Groups["target"].Value.Contains('?') is true)
        {
            target = string.Concat(startLineMatch.Groups["target"].Value.TakeWhile(c => c.Equals('?') is false));

            string[] queryParameterStrings =
                string.Concat(startLineMatch.Groups["target"].Value.SkipWhile(c => c.Equals('?') is false).Skip(1))
                .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string queryParameterString in queryParameterStrings)
            {
                Match queryParameterMatch = Regex.Match(queryParameterString, QueryParameterPattern);

                if (queryParameterMatch.Success is false)
                    throw new ArgumentException("Parse error: one or more query parameters do not match the pattern.",
                        nameof(request));

                queryParameters.Add(new HttpRequestQueryParameter(
                    queryParameterMatch.Groups["parameter"].Value,
                    queryParameterMatch.Groups["value"].Value));
            }
        }
        else
            target = startLineMatch.Groups["target"].Value;

        string protocolVersion = startLineMatch.Groups["protocolVersion"].Value;

        List<HttpHeader> headers = new();

        foreach (Match headerMatch in headerMatches)
            headers.Add(new HttpHeader(
                HttpHeaderGroup.Unknown,
                headerMatch.Groups["parameter"].Value,
                headerMatch.Groups["value"].Value));

        return new HttpRequest(method, target, queryParameters, protocolVersion, headers, body);
    }
}