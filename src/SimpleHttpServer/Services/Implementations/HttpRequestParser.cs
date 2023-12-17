using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;
using SimpleHttpServer.Resources.Entities.Components.HttpHeader;
using SimpleHttpServer.Resources.Services.HttpRequestParser.ExceptionMessages;
using SimpleHttpServer.Services.Abstractions;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using Validation.Helpers;

namespace SimpleHttpServer.Services.Implementations;

/// <summary>
/// Provides a method used to parse HTTP requests.
/// </summary>
public sealed class HttpRequestParser : IHttpRequestParser
{
    private const string StartLinePattern =
        $@"^(?<{MethodPatternGroup}>\w+)\s(?<{TargetPatternGroup}>.+)\sHTTP/(?<{ProtocolVersionPatternGroup}>\d(.\d)*)$";
    private const string QueryParameterPattern =
        $@"^(?<{QueryParameterNamePatternGroup}>.+)=(?<{QueryParameterValuePatternGroup}>.+)$";
    private const string HeaderPattern =
        $@"^(?<{HeaderParameterPatternGroup}>.+):\s(?<{HeaderValuePatternGroup}>.+)$";

    private const string MethodPatternGroup = "method";
    private const string TargetPatternGroup = "target";
    private const string QueryParameterNamePatternGroup = "name";
    private const string QueryParameterValuePatternGroup = "value";
    private const string ProtocolVersionPatternGroup = "protocol";
    private const string HeaderParameterPatternGroup = "parameter";
    private const string HeaderValuePatternGroup = "value";

    private readonly ResourceManager _headerGroupDetailsResourceManager = new(typeof(HttpHeaderGroupDetails));

    /// <summary>
    /// Parses an HTTP request from a sequence of bytes.
    /// </summary>
    /// <param name="request">The sequence of bytes containing the HTTP request.</param>
    /// <returns>A new instance of the <see cref="HttpRequest"/> class.</returns>
    /// <exception cref="ArgumentException"></exception>
    public HttpRequest Parse(IEnumerable<byte> request)
    {
        Verify.NotNull(request);

        string requestString = Encoding.UTF8.GetString(request.ToArray());
        StringReader requestStringReader = new(requestString);
        int metadataLength = 0;
        string? currentLine;

        currentLine = requestStringReader.ReadLine() ??
            throw new ArgumentException(HttpRequestParserExceptionMessages.StartLineNotFound, nameof(request));

        metadataLength += currentLine.Length;

        Match startLineMatch = Regex.Match(currentLine, StartLinePattern);

        if (startLineMatch.Success is false)
            throw new ArgumentException(HttpRequestParserExceptionMessages.StartLineMismatch, nameof(request));

        if (Enum.TryParse(startLineMatch.Groups[MethodPatternGroup].Value, out HttpRequestMethod method) is false)
            method = HttpRequestMethod.NOTIMPLEMENTED;

        string target;
        List<HttpRequestQueryParameter> queryParameters = new();

        if (startLineMatch.Groups[TargetPatternGroup].Value.Contains('?', StringComparison.Ordinal) is false)
            target = startLineMatch.Groups[TargetPatternGroup].Value;
        else
        {
            target = string.Concat(startLineMatch.Groups[TargetPatternGroup].Value.TakeWhile(c => c is not '?'));

            string[] queryParameterStrings = string
                .Concat(startLineMatch.Groups[TargetPatternGroup].Value.SkipWhile(c => c is not '?').Skip(1))
                .Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (string queryParameterString in queryParameterStrings)
            {
                Match queryParameterMatch = Regex.Match(queryParameterString, QueryParameterPattern);

                if (queryParameterMatch.Success is false)
                    throw new ArgumentException(HttpRequestParserExceptionMessages.QueryParameterMismatch, nameof(request));

                queryParameters.Add(new HttpRequestQueryParameter(
                    queryParameterMatch.Groups[QueryParameterNamePatternGroup].Value,
                    queryParameterMatch.Groups[QueryParameterValuePatternGroup].Value));
            }
        }

        string protocolVersion = startLineMatch.Groups[ProtocolVersionPatternGroup].Value;
        List<HttpHeader> headers = new();

        while ((currentLine = requestStringReader.ReadLine()) is not null)
        {
            metadataLength +=
                requestString.Skip(metadataLength).TakeWhile(c => char.IsWhiteSpace(c) is true).Count() +
                currentLine.Length;

            if (currentLine is "")
                break;

            Match headerMatch = Regex.Match(currentLine, HeaderPattern);

            if (headerMatch.Success is false)
                throw new ArgumentException(HttpRequestParserExceptionMessages.HeaderMismatch, nameof(request));

            string? headerGroupDetailsResourceString = _headerGroupDetailsResourceManager
                .GetString(headerMatch.Groups[HeaderParameterPatternGroup].Value);

            if (Enum.TryParse(headerGroupDetailsResourceString, out HttpHeaderGroup headerGroup) is false)
                headerGroup = HttpHeaderGroup.Request;

            headers.Add(new HttpHeader(
                headerGroup,
                headerMatch.Groups[HeaderParameterPatternGroup].Value,
                headerMatch.Groups[HeaderValuePatternGroup].Value));
        }

        int metadataByteCount = Encoding.UTF8.GetByteCount(requestString.Take(metadataLength).ToArray());
        IEnumerable<byte>? body = (metadataByteCount < request.Count()) ? request.Skip(metadataByteCount) : default;

        return new HttpRequest(request, method, target, queryParameters, protocolVersion, headers, body);
    }
}