namespace BridgePay.Domain.Exceptions;

/// <summary>
/// Exception thrown when a transaction validation fails in the domain.
/// </summary>
public class InvalidTransactionException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the InvalidTransactionException class.
    /// </summary>
    public InvalidTransactionException(string message) : base(message) { }
}
