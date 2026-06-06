namespace BridgePay.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BridgePay.Domain.Exceptions;

/// <summary>
/// Represents a merchant in the BridgePay system.
/// </summary>
public class Merchant
{
    /// <summary>
    /// Gets the unique identifier for the merchant.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the user ID from Supabase authentication linked to this merchant.
    /// </summary>
    public Guid AuthUserId { get; private set; }

    /// <summary>
    /// Gets the merchant's registered business name.
    /// </summary>
    public string BusinessName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the merchant's contact email.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the primary API key for this merchant.
    /// </summary>
    public string ApiKey { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the API secret used for signature validation.
    /// </summary>
    public string ApiSecret { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the merchant is active and allowed to process payments.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the UTC date and time when the merchant was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the collection of transactions associated with this merchant.
    /// </summary>
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    /// <summary>
    /// Gets the collection of api keys associated with this merchant.
    /// </summary>
    public ICollection<ApiKey> ApiKeys { get; private set; } = new List<ApiKey>();

    /// <summary>
    /// Parameterless constructor for EF Core serialization.
    /// </summary>
    private Merchant() { }

    /// <summary>
    /// Creates a new Merchant instance.
    /// </summary>
    public static Merchant Create(Guid authUserId, string businessName, string email)
    {
        if (string.IsNullOrWhiteSpace(businessName))
            throw new DomainException("Business name is required.");

        var merchant = new Merchant
        {
            Id = Guid.NewGuid(),
            AuthUserId = authUserId,
            BusinessName = businessName,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        merchant.GenerateApiCredentials();
        return merchant;
    }

    /// <summary>
    /// Generates new API keys for this merchant.
    /// </summary>
    public void GenerateApiCredentials()
    {
        ApiKey = $"bp_live_{Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)).Replace("+", "").Replace("/", "").Replace("=", "")}";
        ApiSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    /// <summary>
    /// Updates the merchant's business name.
    /// </summary>
    public void UpdateBusinessName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Business name cannot be empty.");
        BusinessName = name;
    }

    /// <summary>
    /// Activates the merchant account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the merchant account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}
