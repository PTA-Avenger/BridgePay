namespace BridgePay.Infrastructure.Messaging.Publishers;

using System.Threading;
using System.Threading.Tasks;
using BridgePay.Application.Common.Interfaces;
using MassTransit;

/// <summary>
/// MassTransit implementation of the IMessagePublisher interface for raising integration events.
/// </summary>
public class TransactionPublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>Initializes a new instance of TransactionPublisher.</summary>
    public TransactionPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>Publishes an integration event to RabbitMQ via MassTransit.</summary>
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await _publishEndpoint.Publish(message, cancellationToken);
    }
}
