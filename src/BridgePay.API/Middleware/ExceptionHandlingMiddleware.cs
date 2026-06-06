namespace BridgePay.API.Middleware;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/// <summary>
/// Global exception handling middleware that captures exceptions and translates them to standard RFC 7807 ProblemDetails responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>Initializes a new instance of ExceptionHandlingMiddleware.</summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware asynchronously.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails();
        int statusCode;

        switch (exception)
        {
            case ValidationException valEx:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "Validation Failed";
                problemDetails.Detail = valEx.Message;
                problemDetails.Extensions["errors"] = valEx.Errors;
                break;

            case NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = notFoundEx.Message;
                break;

            case MerchantNotFoundException merchantEx:
                statusCode = StatusCodes.Status404NotFound;
                problemDetails.Title = "Merchant Not Found";
                problemDetails.Detail = merchantEx.Message;
                break;

            case DomainException domEx:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = domEx.Message;
                break;

            case ForbiddenException forbiddenEx:
                statusCode = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Forbidden";
                problemDetails.Detail = forbiddenEx.Message;
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred on the server.";
                break;
        }

        context.Response.StatusCode = statusCode;
        problemDetails.Status = statusCode;

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResult = JsonSerializer.Serialize(problemDetails, jsonOptions);

        await context.Response.WriteAsync(jsonResult);
    }
}
