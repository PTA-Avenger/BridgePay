namespace BridgePay.Domain.Exceptions;

/// <summary>
/// Exception thrown when a payment fails due to insufficient funds.
/// </summary>
public class InsufficientFundsException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the InsufficientFundsException class.
    /// </summary>
    public InsufficientFundsException(string message) : base(message) { }
}
