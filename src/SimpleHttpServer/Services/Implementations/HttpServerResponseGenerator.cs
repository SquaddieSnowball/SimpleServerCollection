using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;
using SimpleHttpServer.Resources.Entities.HttpResponse;
using SimpleHttpServer.Resources.Entities.HttpResponse.Details;
using SimpleHttpServer.Resources.Services.HttpServerResponseGenerator.ExceptionMessages;
using SimpleHttpServer.Resources.Services.HttpServerResponseGenerator.Pages;
using SimpleHttpServer.Services.Abstractions;
using System.Resources;
using System.Text;
using Validation.Helpers;

namespace SimpleHttpServer.Services.Implementations;

/// <summary>
/// Provides functionality for generating HTTP server responses.
/// </summary>
public sealed class HttpServerResponseGenerator : IHttpServerResponseGenerator
{
    private const string LanguagePageTemplatePlaceholderName = "language";
    private const string StatusCodeMessagePageTemplatePlaceholderName = "status-code-message";
    private const string DetailsPageTemplatePlaceholderName = "details";
    private const string ServerNamePageTemplatePlaceholderName = "server-name";

    private readonly ResourceManager _responseStatusMessagesResourceManager = new(typeof(HttpResponseStatusMessages));
    private readonly ResourceManager _responseDetailsResourceManager = new(typeof(HttpResponseDetails));

    /// <summary>
    /// Gets or sets an instance of the <see cref="HttpServer"/> class for which responses will be generated.
    /// </summary>
    public HttpServer? Server { get; set; }

    /// <summary>
    /// Generates an HTTP response for the specified status.
    /// </summary>
    /// <param name="status">Response status.</param>
    /// <param name="requiredHeaders">HTTP headers required to generate a response.</param>
    /// <returns>A new instance of the <see cref="HttpResponse"/> class.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public HttpResponse GenerateResponse(HttpResponseStatus status, IEnumerable<HttpHeader>? requiredHeaders = default)
    {
        if (Server is null)
            throw new InvalidOperationException(HttpServerResponseGeneratorExceptionMessages.ServerNullInstance);

        Verify.EnumerationValue(status);

        string language = Thread.CurrentThread.CurrentUICulture.ToString();

        string statusCode = ((int)status).ToString();
        string statusMessage = _responseStatusMessagesResourceManager.GetString(statusCode) ??
            throw new NotImplementedException(HttpServerResponseGeneratorExceptionMessages.ResponseGenerationNotImplemented);

        string statusCodeMessage = $"{statusCode} {statusMessage}";

        string details = _responseDetailsResourceManager.GetString(statusCode) ??
            throw new NotImplementedException(HttpServerResponseGeneratorExceptionMessages.ResponseGenerationNotImplemented);

        string serverName = Server.Name;

        string page = FillResponsePageTemplatePlaceholders(
            new Dictionary<string, string>()
            {
                { LanguagePageTemplatePlaceholderName, language },
                { StatusCodeMessagePageTemplatePlaceholderName, statusCodeMessage },
                { DetailsPageTemplatePlaceholderName, details },
                { ServerNamePageTemplatePlaceholderName, serverName }
            });

        HttpResponse response = new(status, Enumerable.Empty<HttpHeader>(), Encoding.UTF8.GetBytes(page));

        AttachGeneralHeaders(response);

        if (requiredHeaders?.Any() is true)
            AttachHeaders(response, requiredHeaders, true);

        VerifyHeaders(response, nameof(requiredHeaders));

        return response;
    }

    private static string FillResponsePageTemplatePlaceholders(Dictionary<string, string> placeholderNameValues)
    {
        string page = HttpServerResponseGeneratorPages.ResponsePageTemplate;

        foreach (KeyValuePair<string, string> placeholderNameValue in placeholderNameValues)
            page = page.Replace($"%{placeholderNameValue.Key}%", placeholderNameValue.Value);

        return page;
    }

    private void AttachGeneralHeaders(HttpResponse response) => AttachHeaders(
        response,
        Server!.DefaultHeaders
            .Concat(new HttpHeader[]
            {
                new(HttpHeaderGroup.Representation, "Content-Type", "text/html; charset=utf-8"),
                new(HttpHeaderGroup.Payload, "Content-Length", response.Body!.Count().ToString())
            }),
        false);

    private static void AttachHeaders(HttpResponse response, IEnumerable<HttpHeader> headers, bool nonExistent) =>
        response.Headers = response.Headers
            .Concat(
                (nonExistent is true) ?
                headers.ExceptBy(response.Headers.Select(h => h.Parameter), h => h.Parameter) :
                headers);

    private static void VerifyHeaders(HttpResponse response, string? exceptionParamName = default)
    {
        IEnumerable<string> missingHeaderParameters = response.Status switch
        {
            HttpResponseStatus.Unauthorized => GetMissingHeaderParameters(response, "WWW-Authenticate"),
            HttpResponseStatus.MethodNotAllowed => GetMissingHeaderParameters(response, "Allow"),
            HttpResponseStatus.ProxyAuthenticationRequired => GetMissingHeaderParameters(response, "Proxy-Authenticate"),
            HttpResponseStatus.RequestTimeout => GetMissingHeaderParameters(response, "Connection"),
            HttpResponseStatus.RangeNotSatisfiable => GetMissingHeaderParameters(response, "Content-Range"),
            HttpResponseStatus.UpgradeRequired => GetMissingHeaderParameters(response, "Upgrade", "Connection"),
            HttpResponseStatus.ServiceUnavailable => GetMissingHeaderParameters(response, "Retry-After"),
            _ => Enumerable.Empty<string>()
        };

        if (missingHeaderParameters.Any() is true)
        {
            throw new ArgumentException(
                string.Format(
                    HttpServerResponseGeneratorExceptionMessages.ResponseHeadersMissing,
                    string.Join(", ", missingHeaderParameters)),
                exceptionParamName);
        }
    }

    private static IEnumerable<string> GetMissingHeaderParameters(HttpResponse response, params string[] headerParameters) =>
        headerParameters.Where(
            hp => response.Headers.Any(
                rhp => rhp.Parameter.Equals(hp, StringComparison.Ordinal) is true) is false);
}