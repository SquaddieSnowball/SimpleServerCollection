namespace SimpleHttpServer.Entities.Components;

/// <summary>
/// Represents an HTTP header group.
/// </summary>
public enum HttpHeaderGroup
{
    /// <summary>
    /// Represents the "Request" HTTP header group.
    /// </summary>
    Request,

    /// <summary>
    /// Represents the "Response" HTTP header group.
    /// </summary>
    Response,

    /// <summary>
    /// Represents the "Representation" HTTP header group.
    /// </summary>
    Representation,

    /// <summary>
    /// Represents the "Payload" HTTP header group.
    /// </summary>
    Payload
}