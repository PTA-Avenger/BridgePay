namespace BridgePay.IntegrationTests.Controllers;

using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class HealthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealth_ShouldReturnHealthyStatusAndServices()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var healthData = await response.Content.ReadFromJsonAsync<HealthResponse>();
        healthData.Should().NotBeNull();
        healthData!.Status.Should().Be("Healthy");
        healthData.Version.Should().Be("1.0.0");
        healthData.Services.Should().NotBeNull();
        healthData.Services.Postgres.Should().Be("Connected");
        healthData.Services.RabbitMQ.Should().Be("Connected");
    }

    private class HealthResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public ServicesStatus Services { get; set; } = new();
    }

    private class ServicesStatus
    {
        public string Postgres { get; set; } = string.Empty;
        public string RabbitMQ { get; set; } = string.Empty;
    }
}
