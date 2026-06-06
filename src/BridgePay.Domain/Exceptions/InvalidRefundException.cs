namespace BridgePay.Domain.Exceptions;

/// <summary>
/// Exception thrown when a refund request validation fails in the domain.
/// </summary>
public class InvalidRefundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the InvalidRefundException class.
    /// </summary>
    public InvalidRefundException(string message) : base(message) { }
}
