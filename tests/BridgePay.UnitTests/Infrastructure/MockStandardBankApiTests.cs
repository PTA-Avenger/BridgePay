namespace BridgePay.UnitTests.Infrastructure;

using System;
using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;
using FluentAssertions;
using Xunit;

public class MockStandardBankApiTests
{
    [Fact]
    public async Task SubmitPaymentAsync_ShouldReturnResponse()
    {
        // Arrange
        var api = new MockStandardBankApi();
        var request = new BankTransactionRequest
        {
            TransactionId = Guid.NewGuid(),
            Amount = 100m,
            Currency = "ZAR",
            CardNumber = "411111******1111",
            PaymentMethod = "CreditCard",
            CustomerReference = "REF-1"
        };

        // Act
        var response = await api.SubmitPaymentAsync(request);

        // Assert
        response.Should().NotBeNull();
        if (response.Success)
        {
            response.BankTransactionId.Should().StartWith("STD-");
            response.ErrorCode.Should().BeNull();
            response.ErrorMessage.Should().BeNull();
        }
        else
        {
            response.BankTransactionId.Should().BeNull();
            response.ErrorCode.Should().BeOneOf("INSUFFICIENT_FUNDS", "CARD_EXPIRED", "DECLINED");
            response.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task SubmitRefundAsync_ShouldReturnResponse()
    {
        // Arrange
        var api = new MockStandardBankApi();
        var request = new BankRefundRequest
        {
            RefundId = Guid.NewGuid(),
            Amount = 50m,
            BankTransactionId = "STD-12345",
            Reason = "Customer request"
        };

        // Act
        var response = await api.SubmitRefundAsync(request);

        // Assert
        response.Should().NotBeNull();
        if (response.Success)
        {
            response.BankTransactionId.Should().StartWith("REF-STD-");
            response.ErrorCode.Should().BeNull();
            response.ErrorMessage.Should().BeNull();
        }
        else
        {
            response.BankTransactionId.Should().BeNull();
            response.ErrorCode.Should().Be("REFUND_REJECTED");
            response.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
    }
}
