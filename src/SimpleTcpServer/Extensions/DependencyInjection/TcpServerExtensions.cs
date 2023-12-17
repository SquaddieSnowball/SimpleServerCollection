using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleTcpServer.Extensions.Options;
using Validation.Helpers;

namespace SimpleTcpServer.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for adding TCP server services to <see cref="IServiceCollection"/>.
/// </summary>
public static class TcpServerExtensions
{
    /// <summary>
    /// Adds TCP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configurationSection">The <see cref="IConfigurationSection"/> to configure the server.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddTcpServer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        Verify.NotNull(services);
        Verify.NotNull(configurationSection);

        _ = services
            .AddGeneralServices()
            .Configure<TcpServerOptions>(configurationSection);

        return services;
    }

    /// <summary>
    /// Adds TCP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configurationSectionPath">The configuration section path to bind the underlying options type.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddTcpServer(this IServiceCollection services, string configurationSectionPath)
    {
        Verify.NotNull(services);
        Verify.NotNullOrEmpty(configurationSectionPath);

        _ = services
            .AddGeneralServices()
            .AddOptions<TcpServerOptions>()
            .BindConfiguration(configurationSectionPath);

        return services;
    }

    /// <summary>
    /// Adds TCP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">The <see cref="TcpServerOptions"/> to configure the server.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddTcpServer(this IServiceCollection services, TcpServerOptions options)
    {
        Verify.NotNull(services);
        Verify.NotNull(options);

        _ = services
            .AddGeneralServices()
            .AddOptions<TcpServerOptions>()
            .Configure(
                configureOptions =>
                {
                    configureOptions.IpAddress = options.IpAddress;
                    configureOptions.Port = options.Port;
                    configureOptions.RequestBufferSize = options.RequestBufferSize;
                    configureOptions.RequestReadTimeout = options.RequestReadTimeout;
                });

        return services;
    }

    /// <summary>
    /// Adds TCP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configureOptions">The <see cref="TcpServerOptions"/> delegate to configure the server.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddTcpServer(this IServiceCollection services, Action<TcpServerOptions> configureOptions)
    {
        Verify.NotNull(services);
        Verify.NotNull(configureOptions);

        _ = services
            .AddGeneralServices()
            .Configure(configureOptions);

        return services;
    }

    private static IServiceCollection AddGeneralServices(this IServiceCollection services)
    {
        _ = services
            .AddOptions()
            .AddLogging()
            .AddSingleton<TcpServer>();

        return services;
    }
}