## About

Simple TCP Server is a lightweight TCP server with basic functionality.

## How to use

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
