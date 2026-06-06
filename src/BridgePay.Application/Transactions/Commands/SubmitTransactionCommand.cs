namespace BridgePay.Application.Transactions.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Application.Common.Messages;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Interfaces;
using FluentValidation;
using MediatR;

/// <summary>
/// Command to submit a new transaction.
/// </summary>
public record SubmitTransactionCommand : IRequest<TransactionDto>
{
    /// <summary>Gets the merchant identifier.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the transaction amount.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the currency code (e.g. ZAR).</summary>
    public string Currency { get; init; } = "ZAR";

    /// <summary>Gets the bank provider name (StandardBank, FNB, Absa).</summary>
    public string BankProvider { get; init; } = string.Empty;

    /// <summary>Gets the payment method (CreditCard, DebitCard, EFT, MobilePayment).</summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>Gets the customer reference.</summary>
    public string CustomerReference { get; init; } = string.Empty;
}

/// <summary>
/// Validator for SubmitTransactionCommand.
/// </summary>
public class SubmitTransactionCommandValidator : AbstractValidator<SubmitTransactionCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public SubmitTransactionCommandValidator()
    {
        RuleFor(x => x.MerchantId).NotEmpty().WithMessage("MerchantId is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Currency).NotEmpty().Must(c => Enum.TryParse<Currency>(c, true, out _))
            .WithMessage("Currency must be a supported ISO code (ZAR, USD, EUR, GBP).");
        RuleFor(x => x.BankProvider).NotEmpty().Must(b => Enum.TryParse<BankProvider>(b, true, out _))
            .WithMessage("BankProvider must be a supported provider (StandardBank, FNB, Absa).");
        RuleFor(x => x.PaymentMethod).NotEmpty().Must(p => Enum.TryParse<PaymentMethod>(p, true, out _))
            .WithMessage("PaymentMethod must be a supported method (CreditCard, DebitCard, EFT, MobilePayment).");
        RuleFor(x => x.CustomerReference).NotEmpty().WithMessage("CustomerReference is required.")
            .MaximumLength(50).WithMessage("CustomerReference cannot exceed 50 characters.");
    }
}

/// <summary>
/// Handler for SubmitTransactionCommand.
/// </summary>
public class SubmitTransactionCommandHandler : IRequestHandler<SubmitTransactionCommand, TransactionDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public SubmitTransactionCommandHandler(
        IMerchantRepository merchantRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher,
        IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    /// <summary>Handles transaction submission.</summary>
    public async Task<TransactionDto> Handle(SubmitTransactionCommand request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.MerchantId);
        }

        if (!merchant.IsActive)
        {
            throw new ForbiddenException("Merchant account is inactive.");
        }

        var bankProvider = Enum.Parse<BankProvider>(request.BankProvider, true);
        var paymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod, true);

        var transaction = Transaction.Create(
            request.MerchantId,
            request.Amount,
            request.Currency.ToUpperInvariant(),
            bankProvider,
            paymentMethod.ToString(),
            request.CustomerReference);

        // Determine fee percentage and flat fee based on bank provider
        var (feePct, flatFee) = bankProvider switch
        {
            BankProvider.FNB => (1.5m, 1.50m),
            BankProvider.Absa => (2.0m, 2.50m),
            _ => (1.8m, 2.00m) // StandardBank and defaults
        };

        transaction.CalculateFee(feePct, flatFee);

        await _transactionRepository.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish to queue for asynchronous processing by bank mock
        var submittedEvent = new TransactionSubmitted
        {
            TransactionId = transaction.Id,
            MerchantId = transaction.MerchantId,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            BankProvider = transaction.BankProvider.ToString(),
            PaymentMethod = transaction.PaymentMethod,
            CustomerReference = transaction.CustomerReference
        };

        await _messagePublisher.PublishAsync(submittedEvent, cancellationToken);

        return _mapper.Map<TransactionDto>(transaction);
    }
}
