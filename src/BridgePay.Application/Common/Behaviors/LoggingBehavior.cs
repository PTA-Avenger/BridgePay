namespace BridgePay.Application.Common.Behaviors;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// MediatR pipeline behavior that logs incoming requests and outgoing responses.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the LoggingBehavior class.
    /// </summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles logging of request and response cycle.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("BridgePay Request: {Name} {@Request}", requestName, request);

        var response = await next();

        _logger.LogInformation("BridgePay Response: {Name} {@Response}", requestName, response);

        return response;
    }
}
