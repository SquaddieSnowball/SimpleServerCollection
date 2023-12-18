# Simple Server Collection

![CI](https://github.com/SquaddieSnowball/SimpleServerCollection/actions/workflows/ci.yml/badge.svg)
![CD](https://github.com/SquaddieSnowball/SimpleServerCollection/actions/workflows/cd.yml/badge.svg)

**Simple Server Collection** is a set of lightweight servers with basic functionality. The set includes:

- [TCP server](src/SimpleTcpServer)
- [HTTP server](src/SimpleHttpServer)

## How to install

Servers are distributed as NuGet packages and can be installed using the command line.

- TCP server:

```
dotnet add package SquaddieSnowball.SimpleServerCollection.SimpleTcpServer
```

- HTTP server:

```
dotnet add package SquaddieSnowball.SimpleServerCollection.SimpleHttpServer
```

## How to use

- [TCP server](docs/tcp/README.md)
- [HTTP server](docs/http/README.md)

## License

**Simple Server Collection** is licensed under the [MIT license](LICENSE.txt).
