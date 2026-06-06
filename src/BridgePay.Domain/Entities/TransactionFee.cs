namespace BridgePay.Domain.Entities;

using System;
using BridgePay.Domain.Enums;

/// <summary>
/// Represents a configuration for fees charged based on bank provider and transaction details.
/// </summary>
public class TransactionFee
{
    /// <summary>
    /// Gets the unique identifier for the transaction fee rule.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the bank provider to which this fee rule applies.
    /// </summary>
    public BankProvider BankProvider { get; private set; }

    /// <summary>
    /// Gets the type of transaction (e.g., CreditCard, DebitCard, EFT).
    /// </summary>
    public string TransactionType { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the percentage of the transaction amount charged as a fee.
    /// </summary>
    public decimal FeePercentage { get; private set; }

    /// <summary>
    /// Gets the flat fee charge added to the percentage fee.
    /// </summary>
    public decimal FlatFee { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core serialization.
    /// </summary>
    private TransactionFee() { }

    /// <summary>
    /// Creates a new TransactionFee configuration rule.
    /// </summary>
    public static TransactionFee Create(BankProvider bank, string txType, decimal pct, decimal flat)
    {
        return new TransactionFee
        {
            Id = Guid.NewGuid(),
            BankProvider = bank,
            TransactionType = txType,
            FeePercentage = pct,
            FlatFee = flat
        };
    }
}
