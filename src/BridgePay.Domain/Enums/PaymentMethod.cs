namespace BridgePay.Domain.Enums;

/// <summary>
/// Represents the supported payment methods for transactions.
/// </summary>
public enum PaymentMethod
{
    /// <summary>Payment via credit card (Visa, Mastercard, etc.).</summary>
    CreditCard = 0,

    /// <summary>Payment via debit card.</summary>
    DebitCard = 1,

    /// <summary>Electronic Funds Transfer (bank-to-bank).</summary>
    EFT = 2,

    /// <summary>Mobile payment (e.g., SnapScan, Zapper).</summary>
    MobilePayment = 3
}
