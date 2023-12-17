namespace SimpleHttpServer.Entities.Components;

/// <summary>
/// Represents the status of an HTTP response.
/// </summary>
public enum HttpResponseStatus
{
    /// <summary>
    /// Represents the HTTP response status "100 Continue".
    /// </summary>
    Continue = 100,

    /// <summary>
    /// Represents the HTTP response status "101 Switching Protocols".
    /// </summary>
    SwitchingProtocols = 101,

    /// <summary>
    /// Represents the HTTP response status "200 OK".
    /// </summary>
    OK = 200,

    /// <summary>
    /// Represents the HTTP response status "201 Created".
    /// </summary>
    Created = 201,

    /// <summary>
    /// Represents the HTTP response status "202 Accepted".
    /// </summary>
    Accepted = 202,

    /// <summary>
    /// Represents the HTTP response status "203 Non-Authoritative Information".
    /// </summary>
    NonAuthoritativeInformation = 203,

    /// <summary>
    /// Represents the HTTP response status "204 No Content".
    /// </summary>
    NoContent = 204,

    /// <summary>
    /// Represents the HTTP response status "205 Reset Content".
    /// </summary>
    ResetContent = 205,

    /// <summary>
    /// Represents the HTTP response status "206 Partial Content".
    /// </summary>
    PartialContent = 206,

    /// <summary>
    /// Represents the HTTP response status "300 Multiple Choices".
    /// </summary>
    MultipleChoices = 300,

    /// <summary>
    /// Represents the HTTP response status "301 Moved Permanently".
    /// </summary>
    MovedPermanently = 301,

    /// <summary>
    /// Represents the HTTP response status "302 Found".
    /// </summary>
    Found = 302,

    /// <summary>
    /// Represents the HTTP response status "303 See Other".
    /// </summary>
    SeeOther = 303,

    /// <summary>
    /// Represents the HTTP response status "304 Not Modified".
    /// </summary>
    NotModified = 304,

    /// <summary>
    /// Represents the HTTP response status "307 Temporary Redirect".
    /// </summary>
    TemporaryRedirect = 307,

    /// <summary>
    /// Represents the HTTP response status "308 Permanent Redirect".
    /// </summary>
    PermanentRedirect = 308,

    /// <summary>
    /// Represents the HTTP response status "400 Bad Request".
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// Represents the HTTP response status "401 Unauthorized".
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// Represents the HTTP response status "403 Forbidden".
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// Represents the HTTP response status "404 Not Found".
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// Represents the HTTP response status "405 Method Not Allowed".
    /// </summary>
    MethodNotAllowed = 405,

    /// <summary>
    /// Represents the HTTP response status "406 Not Acceptable".
    /// </summary>
    NotAcceptable = 406,

    /// <summary>
    /// Represents the HTTP response status "407 Proxy Authentication Required".
    /// </summary>
    ProxyAuthenticationRequired = 407,

    /// <summary>
    /// Represents the HTTP response status "408 Request Timeout".
    /// </summary>
    RequestTimeout = 408,

    /// <summary>
    /// Represents the HTTP response status "409 Conflict".
    /// </summary>
    Conflict = 409,

    /// <summary>
    /// Represents the HTTP response status "410 Gone".
    /// </summary>
    Gone = 410,

    /// <summary>
    /// Represents the HTTP response status "411 Length Required".
    /// </summary>
    LengthRequired = 411,

    /// <summary>
    /// Represents the HTTP response status "412 Precondition Failed".
    /// </summary>
    PreconditionFailed = 412,

    /// <summary>
    /// Represents the HTTP response status "413 Content Too Large".
    /// </summary>
    ContentTooLarge = 413,

    /// <summary>
    /// Represents the HTTP response status "414 URI Too Long".
    /// </summary>
    UriTooLong = 414,

    /// <summary>
    /// Represents the HTTP response status "415 Unsupported Media Type".
    /// </summary>
    UnsupportedMediaType = 415,

    /// <summary>
    /// Represents the HTTP response status "416 Range Not Satisfiable".
    /// </summary>
    RangeNotSatisfiable = 416,

    /// <summary>
    /// Represents the HTTP response status "417 Expectation Failed".
    /// </summary>
    ExpectationFailed = 417,

    /// <summary>
    /// Represents the HTTP response status "421 Misdirected Request".
    /// </summary>
    MisdirectedRequest = 421,

    /// <summary>
    /// Represents the HTTP response status "426 Upgrade Required".
    /// </summary>
    UpgradeRequired = 426,

    /// <summary>
    /// Represents the HTTP response status "428 Precondition Required".
    /// </summary>
    PreconditionRequired = 428,

    /// <summary>
    /// Represents the HTTP response status "429 Too Many Requests".
    /// </summary>
    TooManyRequests = 429,

    /// <summary>
    /// Represents the HTTP response status "431 Request Header Fields Too Large".
    /// </summary>
    RequestHeaderFieldsTooLarge = 431,

    /// <summary>
    /// Represents the HTTP response status "451 Unavailable For Legal Reasons".
    /// </summary>
    UnavailableForLegalReasons = 451,

    /// <summary>
    /// Represents the HTTP response status "500 Internal Server Error".
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// Represents the HTTP response status "501 Not Implemented".
    /// </summary>
    NotImplemented = 501,

    /// <summary>
    /// Represents the HTTP response status "502 Bad Gateway".
    /// </summary>
    BadGateway = 502,

    /// <summary>
    /// Represents the HTTP response status "503 Service Unavailable".
    /// </summary>
    ServiceUnavailable = 503,

    /// <summary>
    /// Represents the HTTP response status "504 Gateway Timeout".
    /// </summary>
    GatewayTimeout = 504,

    /// <summary>
    /// Represents the HTTP response status "505 HTTP Version Not Supported".
    /// </summary>
    HttpVersionNotSupported = 505,

    /// <summary>
    /// Represents the HTTP response status "506 Variant Also Negotiates".
    /// </summary>
    VariantAlsoNegotiates = 506,

    /// <summary>
    /// Represents the HTTP response status "510 Not Extended".
    /// </summary>
    NotExtended = 510,

    /// <summary>
    /// Represents the HTTP response status "511 Network Authentication Required".
    /// </summary>
    NetworkAuthenticationRequired = 511
}