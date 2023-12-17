using SimpleHttpServer.Entities.Components;

namespace SimpleHttpServer.Entities;

/// <summary>
/// Represents an HTTP request.
/// </summary>
/// <param name="RawRequest">Raw request.</param>
/// <param name="Method">Request method.</param>
/// <param name="Target">Target of the request.</param>
/// <param name="QueryParameters">Request query parameters.</param>
/// <param name="ProtocolVersion">Protocol version of the request.</param>
/// <param name="Headers">Request headers.</param>
/// <param name="Body">Request body.</param>
public record class HttpRequest(IEnumerable<byte> RawRequest, HttpRequestMethod Method,
    string Target, IEnumerable<HttpRequestQueryParameter> QueryParameters,
    string ProtocolVersion, IEnumerable<HttpHeader> Headers, IEnumerable<byte>? Body = default);