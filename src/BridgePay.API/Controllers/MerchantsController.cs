namespace BridgePay.API.Controllers;

using System;
using System.Threading.Tasks;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Merchants.Commands;
using BridgePay.Application.Merchants.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Handles merchant account registration and key management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MerchantsController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>Initializes a new instance of MerchantsController.</summary>
    public MerchantsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Registers a new merchant profile in the database.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<MerchantDto>> Register(RegisterMerchantCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Retrieves merchant details by unique ID.</summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<MerchantDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetMerchantByIdQuery { Id = id });
        return Ok(result);
    }

    /// <summary>Retrieves merchant details by Supabase Auth User ID.</summary>
    [HttpGet("auth/{authUserId}")]
    [Authorize]
    public async Task<ActionResult<MerchantDto>> GetByAuthUserId(Guid authUserId)
    {
        var result = await _mediator.Send(new GetMerchantByAuthUserIdQuery { AuthUserId = authUserId });
        return Ok(result);
    }

    /// <summary>Updates merchant business name details.</summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<MerchantDto>> Update(Guid id, UpdateMerchantCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL path does not match body.");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Generates new API keys for the merchant.</summary>
    [HttpPost("{id}/keys")]
    [Authorize]
    public async Task<ActionResult<MerchantDto>> GenerateKeys(Guid id)
    {
        var result = await _mediator.Send(new GenerateApiKeyCommand { MerchantId = id });
        return Ok(result);
    }
}
