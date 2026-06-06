namespace BridgePay.API.Controllers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Refunds.Commands;
using BridgePay.Application.Refunds.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Handles requesting and querying refund records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RefundsController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>Initializes a new instance of RefundsController.</summary>
    public RefundsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Submits a refund request for a completed transaction.</summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RefundDto>> RequestRefund(RequestRefundCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Retrieves details of a refund by ID.</summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RefundDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetRefundByIdQuery { Id = id });
        return Ok(result);
    }

    /// <summary>Retrieves all refunds requested for a specific transaction.</summary>
    [HttpGet("transaction/{transactionId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RefundDto>>> GetByTransactionId(Guid transactionId)
    {
        var result = await _mediator.Send(new GetRefundsByTransactionQuery { TransactionId = transactionId });
        return Ok(result);
    }
}
