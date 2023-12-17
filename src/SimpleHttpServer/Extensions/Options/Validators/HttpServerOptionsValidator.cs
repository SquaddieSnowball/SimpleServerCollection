using Microsoft.Extensions.Options;

namespace SimpleHttpServer.Extensions.Options.Validators;

/// <summary>
/// Represents the type used to validate <see cref="HttpServerOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class HttpServerOptionsValidator : IValidateOptions<HttpServerOptions> { }