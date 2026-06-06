namespace BridgePay.Application.Refunds.Queries;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Interfaces;
using MediatR;

/// <summary>
/// Query to retrieve all refunds associated with a transaction.
/// </summary>
public record GetRefundsByTransactionQuery : IRequest<IEnumerable<RefundDto>>
{
    /// <summary>Gets the transaction identifier.</summary>
    public Guid TransactionId { get; init; }
}

/// <summary>
/// Handler for GetRefundsByTransactionQuery.
/// </summary>
public class GetRefundsByTransactionQueryHandler : IRequestHandler<GetRefundsByTransactionQuery, IEnumerable<RefundDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetRefundsByTransactionQueryHandler(
        ITransactionRepository transactionRepository,
        IRefundRepository refundRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _refundRepository = refundRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<IEnumerable<RefundDto>> Handle(GetRefundsByTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
        if (transaction == null)
        {
            throw new NotFoundException(nameof(Transaction), request.TransactionId);
        }

        var refunds = await _refundRepository.GetByTransactionIdAsync(request.TransactionId);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }
}
