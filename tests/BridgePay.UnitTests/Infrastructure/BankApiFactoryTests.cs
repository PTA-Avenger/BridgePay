namespace BridgePay.UnitTests.Infrastructure;

using System;
using BridgePay.Domain.Enums;
using BridgePay.Infrastructure.ExternalServices.MockBanks;
using FluentAssertions;
using Moq;
using Xunit;

public class BankApiFactoryTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public BankApiFactoryTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
    }

    [Theory]
    [InlineData(BankProvider.StandardBank, typeof(MockStandardBankApi))]
    [InlineData(BankProvider.FNB, typeof(MockFnbApi))]
    [InlineData(BankProvider.Absa, typeof(MockAbsaBankApi))]
    public void GetBankApi_ShouldResolveCorrectBankApi(BankProvider provider, Type expectedType)
    {
        // Arrange
        var mockInstance = Activator.CreateInstance(expectedType);
        _serviceProviderMock.Setup(x => x.GetService(expectedType))
            .Returns(mockInstance);

        var factory = new BankApiFactory(_serviceProviderMock.Object);

        // Act
        var result = factory.GetBankApi(provider);

        // Assert
        result.Should().Be(mockInstance);
    }

    [Fact]
    public void GetBankApi_WithFallback_ShouldReturnNewInstanceWhenServiceNotRegistered()
    {
        // Arrange
        _serviceProviderMock.Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(null!); // Return null, so factory falls back to "new Mock...()"

        var factory = new BankApiFactory(_serviceProviderMock.Object);

        // Act
        var result = factory.GetBankApi(BankProvider.StandardBank);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<MockStandardBankApi>();
    }

    [Fact]
    public void GetBankApi_WithInvalidProvider_ShouldThrowArgumentException()
    {
        // Arrange
        var factory = new BankApiFactory(_serviceProviderMock.Object);
        var invalidProvider = (BankProvider)999;

        // Act
        var act = () => factory.GetBankApi(invalidProvider);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Unsupported bank provider: 999");
    }
}
