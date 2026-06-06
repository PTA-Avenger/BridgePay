namespace BridgePay.Application.Transactions.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
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
/// Query to retrieve aggregated transaction analytics for a merchant.
/// </summary>
public record GetTransactionAnalyticsQuery : IRequest<TransactionAnalyticsDto>
{
    /// <summary>Gets the merchant identifier.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the start date of the analytics window (optional).</summary>
    public DateTime? From { get; init; }

    /// <summary>Gets the end date of the analytics window (optional).</summary>
    public DateTime? To { get; init; }
}

/// <summary>
/// Validator for GetTransactionAnalyticsQuery.
/// </summary>
public class GetTransactionAnalyticsQueryValidator : AbstractValidator<GetTransactionAnalyticsQuery>
{
    /// <summary>Initializes validation rules.</summary>
    public GetTransactionAnalyticsQueryValidator()
    {
        RuleFor(x => x.MerchantId).NotEmpty().WithMessage("MerchantId is required.");
    }
}

/// <summary>
/// Handler for GetTransactionAnalyticsQuery.
/// </summary>
public class GetTransactionAnalyticsQueryHandler : IRequestHandler<GetTransactionAnalyticsQuery, TransactionAnalyticsDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetTransactionAnalyticsQueryHandler(
        IMerchantRepository merchantRepository,
        ITransactionRepository transactionRepository,
        IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<TransactionAnalyticsDto> Handle(GetTransactionAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.MerchantId);
        }

        // Set default date range to last 30 days if not provided
        var fromDate = request.From ?? DateTime.UtcNow.AddDays(-30);
        var toDate = request.To ?? DateTime.UtcNow;

        // Fetch transactions for calculation (limit to 100,000 for safety)
        var transactions = (await _transactionRepository.GetByMerchantIdAsync(
            request.MerchantId,
            1,
            100000,
            null,
            fromDate,
            toDate)).ToList();

        var totalCount = transactions.Count;
        var completedTransactions = transactions.Where(t => t.Status == TransactionStatus.Completed).ToList();
        var totalVolume = completedTransactions.Sum(t => t.Amount);

        var successRate = totalCount > 0
            ? Math.Round((decimal)completedTransactions.Count / totalCount * 100m, 2)
            : 0m;

        var activeRefunds = transactions.Count(t => t.Status == TransactionStatus.Refunded || t.Status == TransactionStatus.PartiallyRefunded);

        // Daily volume aggregation
        var dailyVolumes = transactions
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new DailyVolumeDto
            {
                Date = g.Key,
                Volume = g.Where(t => t.Status == TransactionStatus.Completed).Sum(t => t.Amount),
                Count = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToList();

        // Bank distribution aggregation
        var bankDistribution = transactions
            .GroupBy(t => t.BankProvider)
            .Select(g => new BankDistributionDto
            {
                Bank = g.Key.ToString(),
                Volume = g.Where(t => t.Status == TransactionStatus.Completed).Sum(t => t.Amount),
                Count = g.Count()
            })
            .OrderByDescending(b => b.Volume)
            .ToList();

        // Fetch top 10 recent transactions
        var recentTransactions = transactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .ToList();

        return new TransactionAnalyticsDto
        {
            TotalTransactions = totalCount,
            TotalVolume = totalVolume,
            SuccessRate = successRate,
            ActiveRefunds = activeRefunds,
            DailyVolumes = dailyVolumes,
            BankDistribution = bankDistribution,
            RecentTransactions = _mapper.Map<IEnumerable<TransactionDto>>(recentTransactions)
        };
    }
}
