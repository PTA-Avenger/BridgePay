namespace BridgePay.UnitTests.Domain;

using System;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Exceptions;
using FluentAssertions;
using Xunit;

public class TransactionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnPendingTransaction()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var amount = 150.75m;
        var currency = "ZAR";
        var bankProvider = BankProvider.StandardBank;
        var paymentMethod = "CreditCard";
        var customerReference = "REF-12345";

        // Act
        var transaction = Transaction.Create(merchantId, amount, currency, bankProvider, paymentMethod, customerReference);

        // Assert
        transaction.Should().NotBeNull();
        transaction.Id.Should().NotBeEmpty();
        transaction.MerchantId.Should().Be(merchantId);
        transaction.Amount.Should().Be(amount);
        transaction.Currency.Should().Be(currency);
        transaction.BankProvider.Should().Be(bankProvider);
        transaction.PaymentMethod.Should().Be(paymentMethod);
        transaction.CustomerReference.Should().Be(customerReference);
        transaction.Status.Should().Be(TransactionStatus.Pending);
        transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_WithInvalidAmount_ShouldThrowInvalidTransactionException(decimal invalidAmount)
    {
        // Act
        var act = () => Transaction.Create(Guid.NewGuid(), invalidAmount, "ZAR", BankProvider.StandardBank, "CreditCard", "REF-12345");

        // Assert
        act.Should().Throw<InvalidTransactionException>()
           .WithMessage("Amount must be greater than zero.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidCurrency_ShouldThrowInvalidTransactionException(string? invalidCurrency)
    {
        // Act
        var act = () => Transaction.Create(Guid.NewGuid(), 100m, invalidCurrency!, BankProvider.StandardBank, "CreditCard", "REF-12345");

        // Assert
        act.Should().Throw<InvalidTransactionException>()
           .WithMessage("Currency is required.");
    }

    [Fact]
    public void CalculateFee_ShouldSetFeeAndNetAmount()
    {
        // Arrange
        var transaction = Transaction.Create(Guid.NewGuid(), 1000m, "ZAR", BankProvider.StandardBank, "CreditCard", "REF-123");
        var feePercentage = 2.5m; // 2.5% of 1000 = 25
        var flatFee = 2.0m; // total fee = 27

        // Act
        transaction.CalculateFee(feePercentage, flatFee);

        // Assert
        transaction.FeeAmount.Should().Be(27.0m);
        transaction.NetAmount.Should().Be(973.0m);
    }

    [Fact]
    public void MarkAsProcessing_ShouldChangeStatusToProcessing()
    {
        // Arrange
        var transaction = Transaction.Create(Guid.NewGuid(), 100m, "ZAR", BankProvider.StandardBank, "CreditCard", "REF-123");

        // Act
        transaction.MarkAsProcessing();

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Processing);
    }

    [Fact]
    public void MarkAsCompleted_ShouldChangeStatusToCompletedAndSetProcessedAt()
    {
        // Arrange
        var transaction = Transaction.Create(Guid.NewGuid(), 100m, "ZAR", BankProvider.StandardBank, "CreditCard", "REF-123");
        var bankTxId = "BANK-TX-999";

        // Act
        transaction.MarkAsCompleted(bankTxId);

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Completed);
        transaction.BankTransactionId.Should().Be(bankTxId);
        transaction.ProcessedAt.Should().NotBeNull();
        transaction.ProcessedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarkAsFailed_ShouldChangeStatusToFailedAndSetFailureReason()
    {
        // Arrange
        var transaction = Transaction.Create(Guid.NewGuid(), 100m, "ZAR", BankProvider.StandardBank, "CreditCard", "REF-123");
        var reason = "Insufficient Funds";

        // Act
        transaction.MarkAsFailed(reason);

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Failed);
        transaction.FailureReason.Should().Be(reason);
        transaction.ProcessedAt.Should().NotBeNull();
    }
}
