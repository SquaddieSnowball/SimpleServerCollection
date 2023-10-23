# Simple Server Collection

![CI](https://github.com/SquaddieSnowball/SimpleServerCollection/actions/workflows/ci.yml/badge.svg)
![CD](https://github.com/SquaddieSnowball/SimpleServerCollection/actions/workflows/cd.yml/badge.svg)

Simple Server Collection is a lightweight collection of servers with basic functionality. Currently includes:

- **TCP server**
- **HTTP server**

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/SquaddieSnowball/SimpleServerCollection)

## How to install

Each server is distributed as a NuGet package and can be installed using the command line.

### Install TCP server

```
dotnet add package SquaddieSnowball.SimpleServerCollection.SimpleTcpServer
```

### Install HTTP server

```
dotnet add package SquaddieSnowball.SimpleServerCollection.SimpleHttpServer
```

## How to use

### TCP server

Create a TCP server with a loopback IP address and port number 8080, configure a request handler, and start the server:

```c#
using SimpleTcpServer;
using SimpleTcpServer.Modules.Entities;
using System.Net;

TcpServer tcpServer = new(IPAddress.Loopback, 8080)
{
    RequestHandler = req => new TcpResponse(req.Body.Reverse())
};

tcpServer.Start();
```

### HTTP server

Create an HTTP server with a loopback IP address and port number 8080, map the HTTP GET method, and start the server:

```c#
using SimpleHttpServer;
using SimpleHttpServer.Modules.Entities;
using SimpleHttpServer.Modules.Entities.Components;
using System.Net;

HttpServer httpServer = new(IPAddress.Loopback, 8080);

httpServer.MapGet("/", req => new HttpResponse(req.ProtocolVersion, HttpResponseStatus.OK, Enumerable.Empty<HttpHeader>(), "<p>Hello, world!</p>"));

httpServer.Start();
```

## License

Simple Server Collection is licensed under the [MIT license](LICENSE.TXT).
