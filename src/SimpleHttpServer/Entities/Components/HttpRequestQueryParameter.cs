namespace SimpleHttpServer.Entities.Components;

/// <summary>
/// Represents an HTTP request query parameter.
/// </summary>
/// <param name="Name">Parameter name.</param>
/// <param name="Value">Parameter value.</param>
public record class HttpRequestQueryParameter(string Name, string Value);