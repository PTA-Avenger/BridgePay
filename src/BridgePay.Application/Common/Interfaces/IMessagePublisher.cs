namespace BridgePay.Application.Common.Interfaces;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for publishing events/messages to the message broker.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message asynchronously to the broker.
    /// </summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
