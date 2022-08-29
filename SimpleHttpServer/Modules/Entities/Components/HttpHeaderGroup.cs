namespace SimpleHttpServer.Modules.Entities.Components;

/// <summary>
/// Represents an HTTP header group.
/// </summary>
public enum HttpHeaderGroup
{
    General,
    Request,
    Response,
    Entity,
    Unknown
}