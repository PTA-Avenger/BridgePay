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
/// MassTransit consumer that processes a refund request through the mock bank APIs.
/// </summary>
public class RefundConsumer : IConsumer<RefundRequested>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BankApiFactory _bankApiFactory;
    private readonly IAuditLogService _auditLogService;

    /// <summary>Initializes dependencies.</summary>
    public RefundConsumer(
        ITransactionRepository transactionRepository,
        IRefundRepository refundRepository,
        IUnitOfWork unitOfWork,
        BankApiFactory bankApiFactory,
        IAuditLogService auditLogService)
    {
        _transactionRepository = transactionRepository;
        _refundRepository = refundRepository;
        _unitOfWork = unitOfWork;
        _bankApiFactory = bankApiFactory;
        _auditLogService = auditLogService;
    }

    /// <summary>Consumes and processes the refund request.</summary>
    public async Task Consume(ConsumeContext<RefundRequested> context)
    {
        var message = context.Message;

        var refund = await _refundRepository.GetByIdAsync(message.RefundId);
        if (refund == null) return;

        var transaction = await _transactionRepository.GetByIdAsync(refund.TransactionId);
        if (transaction == null) return;

        // Transition refund to Processing
        refund.MarkAsProcessing();
        await _refundRepository.UpdateAsync(refund);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        await _auditLogService.LogActionAsync(
            message.MerchantId,
            "RefundProcessing",
            $"Refund {refund.Id} for transaction {transaction.Id} transitioned to Processing.",
            correlationId: context.CorrelationId?.ToString());

        // Process refund at bank
        var bankApi = _bankApiFactory.GetBankApi(transaction.BankProvider);
        var processor = new StandardPaymentProcessor(bankApi);

        var bankRefundRequest = new BankRefundRequest
        {
            RefundId = refund.Id,
            BankTransactionId = transaction.BankTransactionId ?? string.Empty,
            Amount = refund.Amount,
            Reason = refund.Reason
        };

        BankRefundResponse bankResponse;
        try
        {
            bankResponse = await processor.ProcessRefundAsync(bankRefundRequest);
        }
        catch (Exception ex)
        {
            bankResponse = new BankRefundResponse
            {
                Success = false,
                ErrorCode = "GATEWAY_ERROR",
                ErrorMessage = $"An internal exception occurred: {ex.Message}"
            };
        }

        if (bankResponse.Success)
        {
            refund.MarkAsCompleted();
            await _refundRepository.UpdateAsync(refund);

            // Update original transaction status (Full vs Partial refund)
            var allRefunds = await _refundRepository.GetByTransactionIdAsync(transaction.Id);
            var totalRefunded = allRefunds
                .Where(r => r.Status == RefundStatus.Completed)
                .Sum(r => r.Amount);

            if (totalRefunded >= transaction.Amount)
            {
                transaction.MarkAsRefunded();
            }
            else
            {
                transaction.MarkAsPartiallyRefunded();
            }

            await _transactionRepository.UpdateAsync(transaction);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            await _auditLogService.LogActionAsync(
                message.MerchantId,
                "RefundCompleted",
                $"Refund {refund.Id} for transaction {transaction.Id} was processed successfully.",
                correlationId: context.CorrelationId?.ToString());
        }
        else
        {
            refund.MarkAsFailed();
            await _refundRepository.UpdateAsync(refund);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            await _auditLogService.LogActionAsync(
                message.MerchantId,
                "RefundFailed",
                $"Refund {refund.Id} for transaction {transaction.Id} failed at the bank. Reason: {bankResponse.ErrorCode} - {bankResponse.ErrorMessage}",
                correlationId: context.CorrelationId?.ToString());
        }
    }
}
