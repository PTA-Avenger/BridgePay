namespace BridgePay.API.Middleware;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BridgePay.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// Middleware that logs incoming HTTP requests and responses, capturing performance metrics and saving audit logs.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>Initializes a new instance of RequestLoggingMiddleware.</summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware asynchronously.</summary>
    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService, IAuditLogService auditLogService)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;

        var request = context.Request;
        var requestPath = request.Path + request.QueryString;
        var method = request.Method;

        // Capture request body (masking sensitive fields is recommended in production)
        request.EnableBuffering();
        var requestBody = string.Empty;
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        // Proceed down the pipeline
        await _next(context);

        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;
        var responseStatusCode = context.Response.StatusCode;

        _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
            method, requestPath, responseStatusCode, elapsedMs, correlationId);

        // Save audit log to MongoDB asynchronously (fire and forget or await, await is safer for guarantees)
        var merchantId = currentUserService.UserId;
        var logDetails = $"HTTP {method} {requestPath} returned status {responseStatusCode} in {elapsedMs}ms. Body: {MaskSensitiveData(requestBody)}";

        try
        {
            await auditLogService.LogActionAsync(
                merchantId,
                "ApiRequest",
                logDetails,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                correlationId);
        }
        catch (Exception ex)
        {
            // Fail-safe logging so audit errors don't crash the api response
            _logger.LogError(ex, "Failed to write audit log to MongoDB.");
        }
    }

    private static string MaskSensitiveData(string rawBody)
    {
        if (string.IsNullOrEmpty(rawBody)) return rawBody;
        // Simple mock masking for demonstration purposes
        if (rawBody.Contains("cardNumber", StringComparison.OrdinalIgnoreCase))
        {
            return "{ masked sensitive request payload }";
        }
        return rawBody;
    }
}
