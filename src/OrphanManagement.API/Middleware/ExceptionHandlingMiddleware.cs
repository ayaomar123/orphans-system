using OrphanManagement.Application.DTOs.Common;
using OrphanManagement.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace OrphanManagement.API.Middleware;

/// <summary>
/// Global exception handling middleware to catch and format all exceptions.
/// Provides consistent error responses across the API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, 
                ApiResponse<object>.ErrorResponse(exception.Message)),
            
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, 
                ApiResponse<object>.ErrorResponse(exception.Message)),
            
            ValidationException validationEx => (HttpStatusCode.BadRequest,
                ApiResponse<object>.ErrorResponse("Validation failed", 
                    validationEx.Errors.Select(e => e.ErrorMessage).ToList())),
            
            DomainException => (HttpStatusCode.BadRequest, 
                ApiResponse<object>.ErrorResponse(exception.Message)),
            
            _ => (HttpStatusCode.InternalServerError, 
                ApiResponse<object>.ErrorResponse("An internal server error occurred. Please try again later."))
        };

        context.Response.StatusCode = (int)response.Item1;

        var json = JsonSerializer.Serialize(response.Item2, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
