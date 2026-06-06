namespace BridgePay.Application.Common.DTOs;

using System;

/// <summary>
/// Data transfer object representing a merchant profile.
/// </summary>
public class MerchantDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the merchant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the business name of the merchant.
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the merchant's contact email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API Key (masked or raw as needed by context).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API Secret (typically omitted or masked for security).
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the merchant is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the merchant account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
