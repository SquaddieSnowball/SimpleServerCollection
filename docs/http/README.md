## How to use

Configure the server in the `appsettings.json` file:

```json
{
    "HttpServerOptions": {
        "IpAddress": "127.0.0.1",
        "Port": 8080
    }
}
```

Available options:

**TCP server**:

- `IpAddress*` - string representing the local IP address;
- `Port*` - port on which requests will be listened;
- `RequestBufferSize` - size of the byte array used as a buffer to store the request;
- `RequestReadTimeout` - timeout (in milliseconds) for reading request data.

**HTTP server**:

- `Name` - server name;
- `TraceEnabled` - value indicating whether the HTTP `TRACE` method is enabled on the server.

> [!IMPORTANT]
> Options marked with `*` are required.

Add the required services to the container using the `AddHttpServer` method, map the HTTP `GET` method and start the server:

```c#
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleHttpServer;
using SimpleHttpServer.Entities;
using SimpleHttpServer.Entities.Components;
using SimpleHttpServer.Extensions.DependencyInjection;
using SimpleHttpServer.Extensions.Options;

HostApplicationBuilder hostApplicationBuilder = Host.CreateApplicationBuilder();
hostApplicationBuilder.Services.AddHttpServer(nameof(HttpServerOptions), nameof(HttpServerOptions));
IHost host = hostApplicationBuilder.Build();

HttpServer httpServer = host.Services.GetRequiredService<HttpServer>();
httpServer.MapGet("/", req => new HttpResponse(HttpResponseStatus.OK, Enumerable.Empty<HttpHeader>()));
httpServer.Start();

await host.StartAsync();
```

## License

Simple Server Collection is licensed under the [MIT license](../../LICENSE.txt).
