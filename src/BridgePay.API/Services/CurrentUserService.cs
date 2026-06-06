namespace BridgePay.API.Services;

using System;
using System.Security.Claims;
using BridgePay.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

/// <summary>
/// Service that extracts the current authenticated merchant/user identity from the HTTP context.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Initializes a new instance of CurrentUserService.</summary>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>Gets the unique identifier of the authenticated user (Supabase 'sub' claim).</summary>
    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var subClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("sub");

            if (Guid.TryParse(subClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }

    /// <summary>Gets a value indicating whether the user is authenticated.</summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
