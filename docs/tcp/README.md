## How to use

Configure the server in the `appsettings.json` file:

```json
{
    "TcpServerOptions": {
        "IpAddress": "127.0.0.1",
        "Port": 8080
    }
}
```

Available options:

- `IpAddress*` - string representing the local IP address;
- `Port*` - port on which requests will be listened;
- `RequestBufferSize` - size of the byte array used as a buffer to store the request;
- `RequestReadTimeout` - timeout (in milliseconds) for reading request data.

> [!IMPORTANT]
> Options marked with `*` are required.

Add the required services to the container using the `AddTcpServer` method, configure the request handler and start the server:

```c#
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleTcpServer;
using SimpleTcpServer.Entities;
using SimpleTcpServer.Extensions.DependencyInjection;
using SimpleTcpServer.Extensions.Options;

HostApplicationBuilder hostApplicationBuilder = Host.CreateApplicationBuilder();
hostApplicationBuilder.Services.AddTcpServer(nameof(TcpServerOptions));
IHost host = hostApplicationBuilder.Build();

TcpServer tcpServer = host.Services.GetRequiredService<TcpServer>();
tcpServer.RequestHandler = req => new TcpResponse(req.Body.Reverse());
tcpServer.Start();

await host.StartAsync();
```

## License

Simple Server Collection is licensed under the [MIT license](../../LICENSE.txt).
