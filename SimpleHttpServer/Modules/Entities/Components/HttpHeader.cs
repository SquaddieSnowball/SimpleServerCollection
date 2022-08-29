namespace SimpleHttpServer.Modules.Entities.Components;

/// <summary>
/// Represents an HTTP header consisting of a header group, parameter and a value.
/// </summary>
public sealed class HttpHeader
{
    /// <summary>
    /// Gets group of an HTTP header.
    /// </summary>
    public HttpHeaderGroup Group { get; }

    /// <summary>
    /// Gets parameter of an HTTP header.
    /// </summary>
    public string Parameter { get; }

    /// <summary>
    /// Gets value of an HTTP header.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the HttpHeader class.
    /// </summary>
    /// <param name="group">Group of an HTTP header.</param>
    /// <param name="parameter">Parameter of an HTTP header.</param>
    /// <param name="value">Value of an HTTP header.</param>
    public HttpHeader(HttpHeaderGroup group, string parameter, string value) =>
        (Group, Parameter, Value) = (group, parameter, value);
}