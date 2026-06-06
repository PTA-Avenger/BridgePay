namespace BridgePay.Application.Common.Interfaces;

using System;

/// <summary>
/// Exposes properties of the current authenticated user context.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the unique identifier of the currently authenticated user (from token 'sub' claim).
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
