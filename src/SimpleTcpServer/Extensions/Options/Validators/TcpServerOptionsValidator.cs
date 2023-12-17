using Microsoft.Extensions.Options;

namespace SimpleTcpServer.Extensions.Options.Validators;

/// <summary>
/// Represents the type used to validate <see cref="TcpServerOptions"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class TcpServerOptionsValidator : IValidateOptions<TcpServerOptions> { }