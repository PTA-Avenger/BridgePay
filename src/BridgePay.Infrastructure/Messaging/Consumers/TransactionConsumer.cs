namespace BridgePay.Infrastructure.Messaging.Consumers;

using System;
using System.Threading.Tasks;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Application.Common.Messages;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Interfaces;
using BridgePay.Infrastructure.ExternalServices.MockBanks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;
using MassTransit;

/// <summary>
/// MassTransit consumer that processes a submitted transaction through the mock bank APIs.
/// </summary>
public class TransactionConsumer : IConsumer<TransactionSubmitted>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BankApiFactory _bankApiFactory;
    private readonly IAuditLogService _auditLogService;
    private readonly IMessagePublisher _messagePublisher;

    /// <summary>Initializes dependencies.</summary>
    public TransactionConsumer(
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        BankApiFactory bankApiFactory,
        IAuditLogService auditLogService,
        IMessagePublisher messagePublisher)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _bankApiFactory = bankApiFactory;
        _auditLogService = auditLogService;
        _messagePublisher = messagePublisher;
    }

    /// <summary>Consumes and processes the submitted transaction.</summary>
    public async Task Consume(ConsumeContext<TransactionSubmitted> context)
    {
        var message = context.Message;

        var transaction = await _transactionRepository.GetByIdAsync(message.TransactionId);
        if (transaction == null) return;

        // Transition to Processing
        transaction.MarkAsProcessing();
        await _transactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        await _auditLogService.LogActionAsync(
            transaction.MerchantId,
            "TransactionProcessing",
            $"Transaction {transaction.Id} transitioned to Processing status.",
            correlationId: context.CorrelationId?.ToString());

        // Call Bank API via Bridge Pattern
        var bankApi = _bankApiFactory.GetBankApi(transaction.BankProvider);
        var processor = new StandardPaymentProcessor(bankApi);

        var bankRequest = new BankTransactionRequest
        {
            TransactionId = transaction.Id,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            PaymentMethod = transaction.PaymentMethod,
            CustomerReference = transaction.CustomerReference
        };

        BankTransactionResponse bankResponse;
        try
        {
            bankResponse = await processor.ProcessTransactionAsync(bankRequest);
        }
        catch (Exception ex)
        {
            bankResponse = new BankTransactionResponse
            {
                Success = false,
                ErrorCode = "GATEWAY_ERROR",
                ErrorMessage = $"An internal exception occurred: {ex.Message}"
            };
        }

        if (bankResponse.Success)
        {
            transaction.MarkAsCompleted(bankResponse.BankTransactionId ?? "UNKNOWN_BANK_ID");
            await _transactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            await _auditLogService.LogActionAsync(
                transaction.MerchantId,
                "TransactionCompleted",
                $"Transaction {transaction.Id} completed successfully with Bank ID {transaction.BankTransactionId}.",
                correlationId: context.CorrelationId?.ToString());

            // Publish TransactionCompleted event
            await _messagePublisher.PublishAsync(new TransactionCompleted
            {
                TransactionId = transaction.Id,
                MerchantId = transaction.MerchantId,
                BankTransactionId = transaction.BankTransactionId!,
                Amount = transaction.Amount,
                FeeAmount = transaction.FeeAmount,
                NetAmount = transaction.NetAmount
            }, context.CancellationToken);
        }
        else
        {
            transaction.MarkAsFailed($"{bankResponse.ErrorCode}: {bankResponse.ErrorMessage}");
            await _transactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            await _auditLogService.LogActionAsync(
                transaction.MerchantId,
                "TransactionFailed",
                $"Transaction {transaction.Id} failed processing. Reason: {transaction.FailureReason}",
                correlationId: context.CorrelationId?.ToString());

            // Publish TransactionFailed event
            await _messagePublisher.PublishAsync(new TransactionFailed
            {
                TransactionId = transaction.Id,
                MerchantId = transaction.MerchantId,
                FailureReason = transaction.FailureReason!
            }, context.CancellationToken);
        }
    }
}
