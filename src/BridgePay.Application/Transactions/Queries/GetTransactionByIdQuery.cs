namespace BridgePay.Application.Transactions.Queries;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Interfaces;
using MediatR;

/// <summary>
/// Query to retrieve a transaction by its ID.
/// </summary>
public record GetTransactionByIdQuery : IRequest<TransactionDto>
{
    /// <summary>Gets the transaction ID.</summary>
    public Guid Id { get; init; }
}

/// <summary>
/// Handler for GetTransactionByIdQuery.
/// </summary>
public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<TransactionDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id);
        if (transaction == null)
        {
            throw new NotFoundException(nameof(Transaction), request.Id);
        }

        return _mapper.Map<TransactionDto>(transaction);
    }
}
