using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleHttpServer.Extensions.Options;
using SimpleHttpServer.Services.Abstractions;
using SimpleHttpServer.Services.Implementations;
using SimpleTcpServer;
using SimpleTcpServer.Extensions.DependencyInjection;
using SimpleTcpServer.Extensions.Options;
using Validation.Helpers;

namespace SimpleHttpServer.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for adding HTTP server services to <see cref="IServiceCollection"/>.
/// </summary>
public static class HttpServerExtensions
{
    /// <summary>
    /// Adds HTTP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tcpServerConfigurationSection">The <see cref="IConfigurationSection"/> 
    /// to configure the TCP server.</param>
    /// <param name="httpServerConfigurationSection">The <see cref="IConfigurationSection"/> 
    /// to configure the HTTP server.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddHttpServer(
        this IServiceCollection services,
        IConfigurationSection tcpServerConfigurationSection,
        IConfigurationSection httpServerConfigurationSection)
    {
        Verify.NotNull(services);
        Verify.NotNull(tcpServerConfigurationSection);
        Verify.NotNull(httpServerConfigurationSection);

        _ = services
            .AddTcpServer(tcpServerConfigurationSection)
            .AddGeneralServices()
            .Configure<HttpServerOptions>(httpServerConfigurationSection);

        return services;
    }

    /// <summary>
    /// Adds HTTP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tcpServerConfigurationSectionPath">The configuration section path 
    /// to bind the underlying TCP server options type.</param>
    /// <param name="httpServerConfigurationSectionPath">The configuration section path 
    /// to bind the underlying HTTP server options type.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddHttpServer(
        this IServiceCollection services,
        string tcpServerConfigurationSectionPath,
        string httpServerConfigurationSectionPath)
    {
        Verify.NotNull(services);
        Verify.NotNullOrEmpty(tcpServerConfigurationSectionPath);
        Verify.NotNullOrEmpty(httpServerConfigurationSectionPath);

        _ = services
            .AddTcpServer(tcpServerConfigurationSectionPath)
            .AddGeneralServices()
            .AddOptions<HttpServerOptions>()
            .BindConfiguration(httpServerConfigurationSectionPath);

        return services;
    }

    /// <summary>
    /// Adds HTTP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tcpServerOptions">The <see cref="TcpServerOptions"/> 
    /// to configure the TCP server.</param>
    /// <param name="httpServerOptions">The <see cref="HttpServerOptions"/> 
    /// to configure the HTTP server.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddHttpServer(
        this IServiceCollection services,
        TcpServerOptions tcpServerOptions,
        HttpServerOptions httpServerOptions)
    {
        Verify.NotNull(services);
        Verify.NotNull(tcpServerOptions);
        Verify.NotNull(httpServerOptions);

        _ = services
            .AddTcpServer(tcpServerOptions)
            .AddGeneralServices()
            .AddOptions<HttpServerOptions>()
            .Configure(
                configureOptions =>
                {
                    configureOptions.Name = httpServerOptions.Name;
                    configureOptions.TraceEnabled = httpServerOptions.TraceEnabled;
                });

        return services;
    }

    /// <summary>
    /// Adds HTTP server services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tcpServerConfigureOptions">The <see cref="TcpServerOptions"/> delegate 
    /// to configure the TCP server.</param>
    /// <param name="httpServerConfigureOptions">The <see cref="HttpServerOptions"/> delegate 
    /// to configure the HTTP server.</param>
    /// <returns>The <see cref="IServiceCollection"/> to which the services were added.</returns>
    public static IServiceCollection AddHttpServer(
        this IServiceCollection services,
        Action<TcpServerOptions> tcpServerConfigureOptions,
        Action<HttpServerOptions> httpServerConfigureOptions)
    {
        Verify.NotNull(services);
        Verify.NotNull(tcpServerConfigureOptions);
        Verify.NotNull(httpServerConfigureOptions);

        _ = services
            .AddTcpServer(tcpServerConfigureOptions)
            .AddGeneralServices()
            .Configure(httpServerConfigureOptions);

        return services;
    }

    private static IServiceCollection AddGeneralServices(this IServiceCollection services)
    {
        _ = services
            .AddOptions()
            .AddLogging(b => b.AddFilter(typeof(TcpServer).FullName, LogLevel.None))
            .AddSingleton<IHttpRequestParser, HttpRequestParser>()
            .AddSingleton<IHttpResponseBuilder, HttpResponseBuilder>()
            .AddSingleton<IHttpServerResponseGenerator, HttpServerResponseGenerator>()
            .AddSingleton<HttpServer>();

        return services;
    }
}