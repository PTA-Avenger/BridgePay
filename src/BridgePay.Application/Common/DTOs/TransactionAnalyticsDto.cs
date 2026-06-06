namespace BridgePay.Application.Common.DTOs;

using System.Collections.Generic;

/// <summary>
/// Data transfer object representing aggregated transaction analytics.
/// </summary>
public class TransactionAnalyticsDto
{
    /// <summary>
    /// Gets or sets the total number of transactions.
    /// </summary>
    public int TotalTransactions { get; set; }

    /// <summary>
    /// Gets or sets the total transaction volume (amount sum).
    /// </summary>
    public decimal TotalVolume { get; set; }

    /// <summary>
    /// Gets or sets the transaction success rate as a percentage (0-100).
    /// </summary>
    public decimal SuccessRate { get; set; }

    /// <summary>
    /// Gets or sets the total count of active/pending refunds.
    /// </summary>
    public int ActiveRefunds { get; set; }

    /// <summary>
    /// Gets or sets the daily transaction volumes for the period.
    /// </summary>
    public IEnumerable<DailyVolumeDto> DailyVolumes { get; set; } = new List<DailyVolumeDto>();

    /// <summary>
    /// Gets or sets the distribution of transactions across banks.
    /// </summary>
    public IEnumerable<BankDistributionDto> BankDistribution { get; set; } = new List<BankDistributionDto>();

    /// <summary>
    /// Gets or sets the most recent transactions.
    /// </summary>
    public IEnumerable<TransactionDto> RecentTransactions { get; set; } = new List<TransactionDto>();
}

/// <summary>
/// Data transfer object representing transaction distribution for a specific bank.
/// </summary>
public class BankDistributionDto
{
    /// <summary>
    /// Gets or sets the bank name.
    /// </summary>
    public string Bank { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total volume of transactions routed to this bank.
    /// </summary>
    public decimal Volume { get; set; }

    /// <summary>
    /// Gets or sets the total number of transactions routed to this bank.
    /// </summary>
    public int Count { get; set; }
}
