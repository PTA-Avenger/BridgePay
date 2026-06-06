namespace BridgePay.Domain.Entities;

using System;
using System.Security.Cryptography;

/// <summary>
/// Represents a merchant API key used for programmatically accessing the BridgePay API.
/// </summary>
public class ApiKey
{
    /// <summary>
    /// Gets the unique identifier for the API key.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the merchant associated with this API key.
    /// </summary>
    public Guid MerchantId { get; private set; }

    /// <summary>
    /// Gets the public key identifier.
    /// </summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the secret key used for signing requests (must be kept confidential).
    /// </summary>
    public string Secret { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this API key is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the UTC date and time when the API key was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the UTC expiration date and time, if any.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// Gets or sets the associated Merchant navigation property.
    /// </summary>
    public Merchant? Merchant { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core serialization.
    /// </summary>
    private ApiKey() { }

    /// <summary>
    /// Creates a new ApiKey instance.
    /// </summary>
    public static ApiKey Create(Guid merchantId, DateTime? expiresAt = null)
    {
        return new ApiKey
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Key = $"bp_key_{Convert.ToBase64String(RandomNumberGenerator.GetBytes(20)).Replace("+", "").Replace("/", "").Replace("=", "")}",
            Secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Revokes the API key, making it inactive.
    /// </summary>
    public void Revoke()
    {
        IsActive = false;
    }
}
