namespace BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Represents the processing response returned by a bank API.
/// </summary>
public record BankTransactionResponse
{
    /// <summary>Gets a value indicating whether the payment succeeded.</summary>
    public bool Success { get; init; }

    /// <summary>Gets the bank-allocated transaction ID if successful.</summary>
    public string? BankTransactionId { get; init; }

    /// <summary>Gets the bank-specific error code if failed.</summary>
    public string? ErrorCode { get; init; }

    /// <summary>Gets the bank-specific error message if failed.</summary>
    public string? ErrorMessage { get; init; }
}
