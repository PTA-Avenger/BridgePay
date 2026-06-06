namespace BridgePay.Application.Transactions.Queries;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Interfaces;
using FluentValidation;
using MediatR;

/// <summary>
/// Query to retrieve paginated and filtered transactions for a merchant.
/// </summary>
public record GetTransactionsByMerchantQuery : IRequest<PagedResultDto<TransactionDto>>
{
    /// <summary>Gets the merchant identifier.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the page number (1-indexed).</summary>
    public int Page { get; init; } = 1;

    /// <summary>Gets the page size.</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>Gets the filter status (optional).</summary>
    public string? Status { get; init; }

    /// <summary>Gets the start date for filtering (optional).</summary>
    public DateTime? From { get; init; }

    /// <summary>Gets the end date for filtering (optional).</summary>
    public DateTime? To { get; init; }
}

/// <summary>
/// Validator for GetTransactionsByMerchantQuery.
/// </summary>
public class GetTransactionsByMerchantQueryValidator : AbstractValidator<GetTransactionsByMerchantQuery>
{
    /// <summary>Initializes validation rules.</summary>
    public GetTransactionsByMerchantQueryValidator()
    {
        RuleFor(x => x.MerchantId).NotEmpty().WithMessage("MerchantId is required.");
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
        RuleFor(x => x.Status).Must(s => string.IsNullOrEmpty(s) || Enum.TryParse<TransactionStatus>(s, true, out _))
            .WithMessage("Status must be a valid transaction status value.");
    }
}

/// <summary>
/// Handler for GetTransactionsByMerchantQuery.
/// </summary>
public class GetTransactionsByMerchantQueryHandler : IRequestHandler<GetTransactionsByMerchantQuery, PagedResultDto<TransactionDto>>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetTransactionsByMerchantQueryHandler(
        IMerchantRepository merchantRepository,
        ITransactionRepository transactionRepository,
        IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<PagedResultDto<TransactionDto>> Handle(GetTransactionsByMerchantQuery request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.MerchantId);
        }

        TransactionStatus? statusFilter = null;
        if (!string.IsNullOrEmpty(request.Status))
        {
            statusFilter = Enum.Parse<TransactionStatus>(request.Status, true);
        }

        var transactions = await _transactionRepository.GetByMerchantIdAsync(
            request.MerchantId,
            request.Page,
            request.PageSize,
            statusFilter,
            request.From,
            request.To);

        var totalCount = await _transactionRepository.CountByMerchantIdAsync(
            request.MerchantId,
            statusFilter,
            request.From,
            request.To);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedResultDto<TransactionDto>
        {
            Data = _mapper.Map<IEnumerable<TransactionDto>>(transactions),
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
