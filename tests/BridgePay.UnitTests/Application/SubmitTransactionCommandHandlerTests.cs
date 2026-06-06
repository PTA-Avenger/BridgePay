namespace BridgePay.UnitTests.Application;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Application.Common.Messages;
using BridgePay.Application.Transactions.Commands;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

public class SubmitTransactionCommandHandlerTests
{
    private readonly Mock<IMerchantRepository> _merchantRepoMock;
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly Mock<IMapper> _mapperMock;

    public SubmitTransactionCommandHandlerTests()
    {
        _merchantRepoMock = new Mock<IMerchantRepository>();
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _messagePublisherMock = new Mock<IMessagePublisher>();
        _mapperMock = new Mock<IMapper>();

        _mapperMock.Setup(x => x.Map<TransactionDto>(It.IsAny<Transaction>()))
            .Returns((Transaction src) => new TransactionDto
            {
                Id = src.Id,
                MerchantId = src.MerchantId,
                Amount = src.Amount,
                Currency = src.Currency,
                BankProvider = src.BankProvider.ToString(),
                PaymentMethod = src.PaymentMethod,
                CustomerReference = src.CustomerReference,
                Status = src.Status.ToString(),
                FeeAmount = src.FeeAmount,
                NetAmount = src.NetAmount
            });
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateTransactionAndPublishEvent()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var merchant = Merchant.Create(Guid.NewGuid(), "Acme Corp", "acme@test.com");
        merchant.Activate();

        _merchantRepoMock.Setup(x => x.GetByIdAsync(merchantId))
            .ReturnsAsync(merchant);

        var command = new SubmitTransactionCommand
        {
            MerchantId = merchantId,
            Amount = 1000m,
            Currency = "ZAR",
            BankProvider = "FNB",
            PaymentMethod = "CreditCard",
            CustomerReference = "TX-100"
        };

        var handler = new SubmitTransactionCommandHandler(
            _merchantRepoMock.Object,
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object,
            _messagePublisherMock.Object,
            _mapperMock.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MerchantId.Should().Be(merchantId);
        result.Amount.Should().Be(1000m);
        result.Currency.Should().Be("ZAR");
        result.BankProvider.Should().Be("FNB");
        result.PaymentMethod.Should().Be("CreditCard");
        result.Status.Should().Be("Pending");

        // FNB has 1.5% fee + R1.50 flat fee => 1000 * 0.015 + 1.50 = R16.50 fee
        result.FeeAmount.Should().Be(16.50m);
        result.NetAmount.Should().Be(983.50m);

        _transactionRepoMock.Verify(x => x.AddAsync(It.Is<Transaction>(t => t.Amount == 1000m)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _messagePublisherMock.Verify(x => x.PublishAsync(It.Is<TransactionSubmitted>(e => e.Amount == 1000m), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentMerchant_ShouldThrowNotFoundException()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        _merchantRepoMock.Setup(x => x.GetByIdAsync(merchantId))
            .ReturnsAsync((Merchant?)null);

        var command = new SubmitTransactionCommand { MerchantId = merchantId };
        var handler = new SubmitTransactionCommandHandler(
            _merchantRepoMock.Object,
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object,
            _messagePublisherMock.Object,
            _mapperMock.Object
        );

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WithInactiveMerchant_ShouldThrowForbiddenException()
    {
        // Arrange
        var merchantId = Guid.NewGuid();
        var merchant = Merchant.Create(Guid.NewGuid(), "Acme Inactive", "inactive@test.com");
        merchant.Deactivate();

        _merchantRepoMock.Setup(x => x.GetByIdAsync(merchantId))
            .ReturnsAsync(merchant);

        var command = new SubmitTransactionCommand { MerchantId = merchantId };
        var handler = new SubmitTransactionCommandHandler(
            _merchantRepoMock.Object,
            _transactionRepoMock.Object,
            _unitOfWorkMock.Object,
            _messagePublisherMock.Object,
            _mapperMock.Object
        );

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Merchant account is inactive.");
    }
}
