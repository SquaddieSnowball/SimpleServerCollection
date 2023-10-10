using SimpleHttpServer.Modules.Entities.Components;

namespace SimpleHttpServer.Modules.Entities;

/// <summary>
/// Represents an HTTP endpoint.
/// </summary>
/// <param name="Target">Endpoint target.</param>
internal record class HttpEndpoint(string Target)
{
    private readonly Dictionary<HttpRequestMethod, Func<HttpRequest, HttpResponse>> _requestMethodHandlerCollection = new();

    /// <summary>
    /// A collection of request methods and their handlers.
    /// </summary>
    public IEnumerable<KeyValuePair<HttpRequestMethod, Func<HttpRequest, HttpResponse>>> RequestMethodHandlerCollection =>
        _requestMethodHandlerCollection;

    /// <summary>
    /// Adds a new request method and its handler.
    /// </summary>
    /// <param name="requestMethod">Request method.</param>
    /// <param name="handler">Request method handler.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddRequestMethodHandler(HttpRequestMethod requestMethod, Func<HttpRequest, HttpResponse> handler)
    {
        try
        {
            _requestMethodHandlerCollection.Add(requestMethod, handler);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("A handler for the specified request method already exists.", nameof(requestMethod));
        }
        catch
        {
            throw;
        }
    }
}