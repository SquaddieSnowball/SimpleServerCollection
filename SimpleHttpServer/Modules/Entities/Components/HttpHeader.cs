namespace SimpleHttpServer.Modules.Entities.Components;

/// <summary>
/// Represents an HTTP header.
/// </summary>
/// <param name="Group">Header group.</param>
/// <param name="Parameter">Header parameter.</param>
/// <param name="Value">Header value.</param>
public record class HttpHeader(HttpHeaderGroup Group, string Parameter, string Value);