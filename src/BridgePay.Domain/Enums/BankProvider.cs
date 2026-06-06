namespace BridgePay.Domain.Enums;

/// <summary>
/// Represents the supported bank providers for payment processing.
/// </summary>
public enum BankProvider
{
    /// <summary>Standard Bank of South Africa.</summary>
    StandardBank = 0,

    /// <summary>First National Bank (FNB).</summary>
    FNB = 1,

    /// <summary>Amalgamated Banks of South Africa (Absa).</summary>
    Absa = 2
}
