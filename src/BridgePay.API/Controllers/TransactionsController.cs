namespace BridgePay.API.Controllers;

using System;
using System.Threading.Tasks;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Transactions.Commands;
using BridgePay.Application.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Handles transaction processing operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ISender _mediator;

    /// <summary>Initializes a new instance of TransactionsController.</summary>
    public TransactionsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Submits a payment transaction for processing.</summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TransactionDto>> Submit(SubmitTransactionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Retrieves details of a single transaction by ID.</summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TransactionDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetTransactionByIdQuery { Id = id });
        return Ok(result);
    }

    /// <summary>Lists transactions for a specific merchant, supporting pagination and filtering.</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResultDto<TransactionDto>>> List(
        [FromQuery] Guid merchantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _mediator.Send(new GetTransactionsByMerchantQuery
        {
            MerchantId = merchantId,
            Page = page,
            PageSize = pageSize,
            Status = status,
            From = from,
            To = to
        });
        return Ok(result);
    }

    /// <summary>Retrieves aggregated transaction analytics metrics.</summary>
    [HttpGet("analytics")]
    [Authorize]
    public async Task<ActionResult<TransactionAnalyticsDto>> GetAnalytics(
        [FromQuery] Guid merchantId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _mediator.Send(new GetTransactionAnalyticsQuery
        {
            MerchantId = merchantId,
            From = from,
            To = to
        });
        return Ok(result);
    }
}
