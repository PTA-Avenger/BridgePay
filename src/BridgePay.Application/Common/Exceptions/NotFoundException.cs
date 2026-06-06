namespace BridgePay.Application.Common.Exceptions;

using System;

/// <summary>
/// Exception thrown when a requested application resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the NotFoundException class.
    /// </summary>
    public NotFoundException() : base() { }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a message.
    /// </summary>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class specifying the resource name and key.
    /// </summary>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}
