namespace BridgePay.Application.Common.Exceptions;

using System;

/// <summary>
/// Exception thrown when a user attempts to perform an action they do not have permission for.
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ForbiddenException class.
    /// </summary>
    public ForbiddenException() : base() { }

    /// <summary>
    /// Initializes a new instance of the ForbiddenException class with a message.
    /// </summary>
    public ForbiddenException(string message) : base(message) { }
}
