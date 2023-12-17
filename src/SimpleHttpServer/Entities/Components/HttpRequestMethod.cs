namespace SimpleHttpServer.Entities.Components;

/// <summary>
/// Represents an HTTP request method.
/// </summary>
public enum HttpRequestMethod
{
    /// <summary>
    /// Represents the HTTP "GET" request method.
    /// </summary>
    GET,

    /// <summary>
    /// Represents the HTTP "HEAD" request method.
    /// </summary>
    HEAD,

    /// <summary>
    /// Represents the HTTP "POST" request method.
    /// </summary>
    POST,

    /// <summary>
    /// Represents the HTTP "PUT" request method.
    /// </summary>
    PUT,

    /// <summary>
    /// Represents the HTTP "PATCH" request method.
    /// </summary>
    PATCH,

    /// <summary>
    /// Represents the HTTP "DELETE" request method.
    /// </summary>
    DELETE,

    /// <summary>
    /// Represents the HTTP "CONNECT" request method.
    /// </summary>
    CONNECT,

    /// <summary>
    /// Represents the HTTP "OPTIONS" request method.
    /// </summary>
    OPTIONS,

    /// <summary>
    /// Represents the HTTP "TRACE" request method.
    /// </summary>
    TRACE,

    /// <summary>
    /// Represents an HTTP request method that is not implemented.
    /// </summary>
    NOTIMPLEMENTED
}