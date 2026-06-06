namespace BridgePay.Application.Refunds.Commands;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Application.Common.Messages;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Exceptions;
using BridgePay.Domain.Interfaces;
using FluentValidation;
using MediatR;

/// <summary>
/// Command to request a refund for a transaction.
/// </summary>
public record RequestRefundCommand : IRequest<RefundDto>
{
    /// <summary>Gets the transaction identifier.</summary>
    public Guid TransactionId { get; init; }

    /// <summary>Gets the refund amount requested.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the reason for refunding.</summary>
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Validator for RequestRefundCommand.
/// </summary>
public class RequestRefundCommandValidator : AbstractValidator<RequestRefundCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public RequestRefundCommandValidator()
    {
        RuleFor(x => x.TransactionId).NotEmpty().WithMessage("TransactionId is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Refund amount must be greater than zero.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters.");
    }
}

/// <summary>
/// Handler for RequestRefundCommand.
/// </summary>
public class RequestRefundCommandHandler : IRequestHandler<RequestRefundCommand, RefundDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public RequestRefundCommandHandler(
        ITransactionRepository transactionRepository,
        IRefundRepository refundRepository,
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _refundRepository = refundRepository;
        _unitOfWork = unitOfWork;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    /// <summary>Handles refund request.</summary>
    public async Task<RefundDto> Handle(RequestRefundCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);
        if (transaction == null)
        {
            throw new NotFoundException(nameof(Transaction), request.TransactionId);
        }

        if (transaction.Status != TransactionStatus.Completed && transaction.Status != TransactionStatus.PartiallyRefunded)
        {
            throw new DomainException("Only completed or partially refunded transactions can be refunded.");
        }

        // Calculate max refundable amount
        var existingRefunds = await _refundRepository.GetByTransactionIdAsync(request.TransactionId);
        var alreadyRefunded = existingRefunds
            .Where(r => r.Status == RefundStatus.Completed || r.Status == RefundStatus.Processing || r.Status == RefundStatus.Pending)
            .Sum(r => r.Amount);

        var maxRefundableAmount = transaction.Amount - alreadyRefunded;

        var refund = Refund.Create(
            request.TransactionId,
            request.Amount,
            request.Reason,
            maxRefundableAmount);

        await _refundRepository.AddAsync(refund);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event to queue for async bank processing
        var refundRequestedEvent = new RefundRequested
        {
            RefundId = refund.Id,
            TransactionId = refund.TransactionId,
            MerchantId = transaction.MerchantId,
            Amount = refund.Amount,
            Reason = refund.Reason
        };

        await _messagePublisher.PublishAsync(refundRequestedEvent, cancellationToken);

        return _mapper.Map<RefundDto>(refund);
    }
}
