namespace SimpleHttpServer.Modules.Entities.Components;

public sealed class HttpRequestQueryParameter
{
    public string Parameter { get; }

    public string Value { get; }

    public HttpRequestQueryParameter(string parameter, string value) =>
        (Parameter, Value) = (parameter, value);
}