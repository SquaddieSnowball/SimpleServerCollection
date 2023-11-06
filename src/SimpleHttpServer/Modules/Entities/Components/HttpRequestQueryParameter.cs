namespace SimpleHttpServer.Modules.Entities.Components;

/// <summary>
/// Represents an HTTP request query parameter.
/// </summary>
/// <param name="Parameter">Parameter name.</param>
/// <param name="Value">Parameter value.</param>
public record class HttpRequestQueryParameter(string Parameter, string Value);