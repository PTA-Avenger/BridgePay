namespace BridgePay.UnitTests.Domain;

using BridgePay.Domain.Enums;
using BridgePay.Domain.Exceptions;
using BridgePay.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldSetAmountAndCurrency()
    {
        // Arrange & Act
        var money = new Money(100.50m, Currency.ZAR);

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be(Currency.ZAR);
    }

    [Fact]
    public void Addition_WithSameCurrency_ShouldSucceed()
    {
        // Arrange
        var m1 = new Money(100m, Currency.ZAR);
        var m2 = new Money(50m, Currency.ZAR);

        // Act
        var result = m1 + m2;

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be(Currency.ZAR);
    }

    [Fact]
    public void Addition_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var m1 = new Money(100m, Currency.ZAR);
        var m2 = new Money(50m, Currency.USD);

        // Act
        var act = () => { _ = m1 + m2; };

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Cannot add amounts with different currencies.");
    }

    [Fact]
    public void Subtraction_WithSameCurrency_ShouldSucceed()
    {
        // Arrange
        var m1 = new Money(100m, Currency.ZAR);
        var m2 = new Money(30m, Currency.ZAR);

        // Act
        var result = m1 - m2;

        // Assert
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be(Currency.ZAR);
    }

    [Fact]
    public void Subtraction_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var m1 = new Money(100m, Currency.ZAR);
        var m2 = new Money(30m, Currency.USD);

        // Act
        var act = () => { _ = m1 - m2; };

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Cannot subtract amounts with different currencies.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeTrue()
    {
        // Arrange
        var m1 = new Money(100m, Currency.ZAR);
        var m2 = new Money(100m, Currency.ZAR);

        // Act & Assert
        m1.Equals(m2).Should().BeTrue();
        (m1 == m2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldBeFalse()
    {
        // Arrange
        var m1 = new Money(100m, Currency.ZAR);
        var m2 = new Money(150m, Currency.ZAR);
        var m3 = new Money(100m, Currency.USD);

        // Act & Assert
        m1.Equals(m2).Should().BeFalse();
        m1.Equals(m3).Should().BeFalse();
        (m1 != m2).Should().BeTrue();
    }
}
