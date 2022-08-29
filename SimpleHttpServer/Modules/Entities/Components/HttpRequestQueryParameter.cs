namespace SimpleHttpServer.Modules.Entities.Components;

/// <summary>
/// Represents an HTTP request query parameter consisting of a parameter name and a value.
/// </summary>
public sealed class HttpRequestQueryParameter
{
    /// <summary>
    /// Gets parameter name of an HTTP request query parameter.
    /// </summary>
    public string Parameter { get; }

    /// <summary>
    /// Gets value of an HTTP request query parameter.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the HttpRequestQueryParameter class.
    /// </summary>
    /// <param name="parameter">Parameter name of an HTTP request query parameter.</param>
    /// <param name="value">Value of an HTTP request query parameter.</param>
    public HttpRequestQueryParameter(string parameter, string value) =>
        (Parameter, Value) = (parameter, value);
}