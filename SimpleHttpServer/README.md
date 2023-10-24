## About

Simple HTTP Server is a lightweight HTTP server with basic functionality.

## How to use

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
