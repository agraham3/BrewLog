using System.Net;
using System.Text.Json;
using Moq;
using Xunit;
using FluentValidation;
using FluentValidation.Results;
using BrewLog.Api.Middleware;
using BrewLog.Api.Services.Exceptions;
using Microsoft.Extensions.Logging;

namespace BrewLog.Api.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
    private readonly DefaultHttpContext _context;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WithValidationException_ReturnsValidationErrorResponse()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new("GrindSize", "Grind size must be between 1 and 30"),
            new("WaterTemperature", "Water temperature must be between 80 and 100 degrees")
        };
        var validationException = new ValidationException(validationFailures);

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw validationException,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, JsonOptions);

        Assert.NotNull(errorResponse);
        Assert.Equal("VALIDATION_ERROR", errorResponse.Error.Code);
        Assert.Equal("One or more validation errors occurred", errorResponse.Error.Message);
        Assert.Equal(2, errorResponse.Error.Details.Count);

        var grindSizeError = errorResponse.Error.Details.First(d => d.Field == "GrindSize");
        Assert.Equal("Grind size must be between 1 and 30", grindSizeError.Message);

        var temperatureError = errorResponse.Error.Details.First(d => d.Field == "WaterTemperature");
        Assert.Equal("Water temperature must be between 80 and 100 degrees", temperatureError.Message);
    }

    [Fact]
    public async Task InvokeAsync_WithNotFoundException_ReturnsNotFoundResponse()
    {
        // Arrange
        var notFoundException = new NotFoundException("CoffeeBean", 123);

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw notFoundException,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, JsonOptions);

        Assert.NotNull(errorResponse);
        Assert.Equal("NOT_FOUND", errorResponse.Error.Code);
        Assert.Equal("CoffeeBean with ID 123 was not found.", errorResponse.Error.Message);
        Assert.Empty(errorResponse.Error.Details);
    }

    [Fact]
    public async Task InvokeAsync_WithBusinessValidationException_ReturnsBadRequestResponse()
    {
        // Arrange
        var businessException = new BusinessValidationException("Cannot delete coffee bean that is referenced by brew sessions");

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw businessException,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, JsonOptions);

        Assert.NotNull(errorResponse);
        Assert.Equal("BUSINESS_VALIDATION_ERROR", errorResponse.Error.Code);
        Assert.Equal("Cannot delete coffee bean that is referenced by brew sessions", errorResponse.Error.Message);
        Assert.Empty(errorResponse.Error.Details);
    }

    [Fact]
    public async Task InvokeAsync_WithReferentialIntegrityException_ReturnsConflictResponse()
    {
        // Arrange
        var referentialException = new ReferentialIntegrityException("Cannot delete entity due to foreign key constraints");

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw referentialException,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Conflict, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, JsonOptions);

        Assert.NotNull(errorResponse);
        Assert.Equal("REFERENTIAL_INTEGRITY_ERROR", errorResponse.Error.Code);
        Assert.Equal("Cannot delete entity due to foreign key constraints", errorResponse.Error.Message);
        Assert.Empty(errorResponse.Error.Details);
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_ReturnsInternalServerErrorResponse()
    {
        // Arrange
        var genericException = new Exception("Something went wrong");

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw genericException,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, _context.Response.StatusCode);
        Assert.Equal("application/json", _context.Response.ContentType);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, JsonOptions);

        Assert.NotNull(errorResponse);
        Assert.Equal("INTERNAL_SERVER_ERROR", errorResponse.Error.Code);
        Assert.Equal("An unexpected error occurred", errorResponse.Error.Message);
        Assert.Empty(errorResponse.Error.Details);
    }

    [Fact]
    public async Task InvokeAsync_WithNoException_CallsNextMiddleware()
    {
        // Arrange
        var nextCalled = false;
        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(200, _context.Response.StatusCode); // Default status code
    }

    [Fact]
    public async Task InvokeAsync_LogsExceptionWhenThrown()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw exception,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unhandled exception occurred")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyValidationException_ReturnsValidationErrorWithEmptyDetails()
    {
        // Arrange
        var validationException = new ValidationException([]);

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw validationException,
            logger: _mockLogger.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, _context.Response.StatusCode);

        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, JsonOptions);

        Assert.NotNull(errorResponse);
        Assert.Equal("VALIDATION_ERROR", errorResponse.Error.Code);
        Assert.Empty(errorResponse.Error.Details);
    }
}