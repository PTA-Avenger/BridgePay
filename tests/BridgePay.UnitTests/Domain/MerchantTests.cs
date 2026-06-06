namespace BridgePay.UnitTests.Domain;

using System;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Exceptions;
using FluentAssertions;
using Xunit;

public class MerchantTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnActiveMerchantWithApiKeys()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var businessName = "Test Business Ltd";
        var email = "merchant@test.com";

        // Act
        var merchant = Merchant.Create(authUserId, businessName, email);

        // Assert
        merchant.Should().NotBeNull();
        merchant.Id.Should().NotBeEmpty();
        merchant.AuthUserId.Should().Be(authUserId);
        merchant.BusinessName.Should().Be(businessName);
        merchant.Email.Should().Be(email);
        merchant.IsActive.Should().BeTrue();
        merchant.ApiKey.Should().StartWith("bp_live_");
        merchant.ApiSecret.Should().NotBeNullOrWhiteSpace();
        merchant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidBusinessName_ShouldThrowDomainException(string? invalidName)
    {
        // Act
        var act = () => Merchant.Create(Guid.NewGuid(), invalidName!, "merchant@test.com");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Business name is required.");
    }

    [Fact]
    public void GenerateApiCredentials_ShouldGenerateNewKeys()
    {
        // Arrange
        var merchant = Merchant.Create(Guid.NewGuid(), "Test Merchant", "merchant@test.com");
        var originalKey = merchant.ApiKey;
        var originalSecret = merchant.ApiSecret;

        // Act
        merchant.GenerateApiCredentials();

        // Assert
        merchant.ApiKey.Should().NotBe(originalKey);
        merchant.ApiKey.Should().StartWith("bp_live_");
        merchant.ApiSecret.Should().NotBe(originalSecret);
    }

    [Fact]
    public void UpdateBusinessName_WithValidName_ShouldUpdate()
    {
        // Arrange
        var merchant = Merchant.Create(Guid.NewGuid(), "Old Name", "merchant@test.com");

        // Act
        merchant.UpdateBusinessName("New Name");

        // Assert
        merchant.BusinessName.Should().Be("New Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateBusinessName_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var merchant = Merchant.Create(Guid.NewGuid(), "Old Name", "merchant@test.com");

        // Act
        var act = () => merchant.UpdateBusinessName(invalidName!);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Business name cannot be empty.");
    }

    [Fact]
    public void Deactivate_ShouldMakeMerchantInactive()
    {
        // Arrange
        var merchant = Merchant.Create(Guid.NewGuid(), "Test Merchant", "merchant@test.com");

        // Act
        merchant.Deactivate();

        // Assert
        merchant.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldMakeMerchantActive()
    {
        // Arrange
        var merchant = Merchant.Create(Guid.NewGuid(), "Test Merchant", "merchant@test.com");
        merchant.Deactivate();

        // Act
        merchant.Activate();

        // Assert
        merchant.IsActive.Should().BeTrue();
    }
}
