namespace BridgePay.Domain.Exceptions;

/// <summary>
/// Exception thrown when a merchant is not found.
/// </summary>
public class MerchantNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the MerchantNotFoundException class.
    /// </summary>
    public MerchantNotFoundException(string message) : base(message) { }
}
