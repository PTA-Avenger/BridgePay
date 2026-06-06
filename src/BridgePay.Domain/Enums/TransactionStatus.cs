namespace BridgePay.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a payment transaction.
/// </summary>
public enum TransactionStatus
{
    /// <summary>Transaction has been created but not yet sent for processing.</summary>
    Pending = 0,

    /// <summary>Transaction has been submitted to the bank and is being processed.</summary>
    Processing = 1,

    /// <summary>Transaction was successfully processed by the bank.</summary>
    Completed = 2,

    /// <summary>Transaction processing failed at the bank or gateway level.</summary>
    Failed = 3,

    /// <summary>Transaction has been fully refunded.</summary>
    Refunded = 4,

    /// <summary>Transaction has been partially refunded.</summary>
    PartiallyRefunded = 5
}
