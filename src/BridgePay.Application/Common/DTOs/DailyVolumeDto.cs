namespace BridgePay.Application.Common.DTOs;

using System;

/// <summary>
/// Data transfer object representing the transaction volume and count for a specific date.
/// </summary>
public class DailyVolumeDto
{
    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the total transaction volume (amount sum) for the date.
    /// </summary>
    public decimal Volume { get; set; }

    /// <summary>
    /// Gets or sets the total number of transactions for the date.
    /// </summary>
    public int Count { get; set; }
}
