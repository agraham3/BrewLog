using System.Net;
using System.Text.Json;
using BrewLog.Api.Services.Exceptions;
using FluentValidation;

namespace BrewLog.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                Error = new ErrorDetails
                {
                    Code = "VALIDATION_ERROR",
                    Message = "One or more validation errors occurred",
                    Details = validationEx.Errors.Select(e => new ValidationError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                }
            },
            BusinessValidationException businessEx => new ErrorResponse
            {
                Error = new ErrorDetails
                {
                    Code = "BUSINESS_VALIDATION_ERROR",
                    Message = businessEx.Message,
                    Details = new List<ValidationError>()
                }
            },
            NotFoundException notFoundEx => new ErrorResponse
            {
                Error = new ErrorDetails
                {
                    Code = "NOT_FOUND",
                    Message = notFoundEx.Message,
                    Details = new List<ValidationError>()
                }
            },
            ReferentialIntegrityException refEx => new ErrorResponse
            {
                Error = new ErrorDetails
                {
                    Code = "REFERENTIAL_INTEGRITY_ERROR",
                    Message = refEx.Message,
                    Details = new List<ValidationError>()
                }
            },
            _ => new ErrorResponse
            {
                Error = new ErrorDetails
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An unexpected error occurred",
                    Details = new List<ValidationError>()
                }
            }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            BusinessValidationException => (int)HttpStatusCode.BadRequest,
            NotFoundException => (int)HttpStatusCode.NotFound,
            ReferentialIntegrityException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public ErrorDetails Error { get; set; } = new();
}

public class ErrorDetails
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ValidationError> Details { get; set; } = new();
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}