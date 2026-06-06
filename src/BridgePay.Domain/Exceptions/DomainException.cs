namespace BridgePay.Domain.Exceptions;

using System;

/// <summary>
/// Base exception for domain errors.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DomainException class.
    /// </summary>
    public DomainException(string message) : base(message) { }
}
