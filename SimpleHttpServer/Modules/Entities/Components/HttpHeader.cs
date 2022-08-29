namespace SimpleHttpServer.Modules.Entities.Components;

public sealed class HttpHeader
{
    public HttpHeaderGroup Group { get; }

    public string Parameter { get; }

    public string Value { get; }

    public HttpHeader(HttpHeaderGroup group, string parameter, string value) =>
        (Group, Parameter, Value) = (group, parameter, value);
}