# Simple Server Collection

![CI](https://github.com/SquaddieSnowball/SimpleServerCollection/actions/workflows/ci.yml/badge.svg)
![CD](https://github.com/SquaddieSnowball/SimpleServerCollection/actions/workflows/cd.yml/badge.svg)

Simple Server Collection is a lightweight collection of servers with basic functionality. Currently includes:

- [TCP server](src/SimpleTcpServer)
- [HTTP server](src/SimpleHttpServer)

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/SquaddieSnowball/SimpleServerCollection)

## How to install

Each server is distributed as a NuGet package and can be installed using the command line:

- Install TCP server:

```
dotnet add package SquaddieSnowball.SimpleServerCollection.SimpleTcpServer
```

- Install HTTP server:

```
dotnet add package SquaddieSnowball.SimpleServerCollection.SimpleHttpServer
```

## How to use

- [TCP server](docs/tcp/README.md)
- [HTTP server](docs/http/README.md)

## License

Simple Server Collection is licensed under the [MIT license](LICENSE.txt).
