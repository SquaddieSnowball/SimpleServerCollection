﻿namespace SimpleHttpServer.Modules.Entities.Components;

/// <summary>
/// Represents an HTTP request method.
/// </summary>
public enum HttpRequestMethod
{
    GET,
    HEAD,
    POST,
    PUT,
    PATCH,
    DELETE,
    OPTIONS,
    TRACE,
    NOTIMPLEMENTED
}