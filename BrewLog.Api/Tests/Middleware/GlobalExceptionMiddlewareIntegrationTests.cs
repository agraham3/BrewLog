using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentValidation;
using FluentValidation.Results;
using BrewLog.Api.Middleware;
using BrewLog.Api.Services.Exceptions;

namespace BrewLog.Api.Tests.Middleware;

public class GlobalExceptionMiddlewareIntegrationTests
{
    [Fact]
    public async Task Middleware_WithValidationException_ProducesCorrectJsonResponse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Brand", "Brand cannot be empty")
        };
        var validationException = new ValidationException(validationFailures);

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw validationException,
            logger: mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        // Verify the JSON structure matches our expected format
        Assert.Contains("\"code\":\"VALIDATION_ERROR\"", responseBody);
        Assert.Contains("\"message\":\"One or more validation errors occurred\"", responseBody);
        Assert.Contains("\"field\":\"Name\"", responseBody);
        Assert.Contains("\"field\":\"Brand\"", responseBody);
        Assert.Contains("Name is required", responseBody);
        Assert.Contains("Brand cannot be empty", responseBody);
    }

    [Fact]
    public async Task Middleware_WithNotFoundException_ProducesCorrectJsonResponse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var notFoundException = new NotFoundException("CoffeeBean", 123);

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw notFoundException,
            logger: mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        // Verify the JSON structure matches our expected format
        Assert.Contains("\"code\":\"NOT_FOUND\"", responseBody);
        Assert.Contains("CoffeeBean with ID 123 was not found.", responseBody);
    }

    [Fact]
    public async Task Middleware_WithBusinessValidationException_ProducesCorrectJsonResponse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var businessException = new BusinessValidationException("Cannot delete coffee bean with active brew sessions");

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw businessException,
            logger: mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        // Verify the JSON structure matches our expected format
        Assert.Contains("\"code\":\"BUSINESS_VALIDATION_ERROR\"", responseBody);
        Assert.Contains("Cannot delete coffee bean with active brew sessions", responseBody);
    }

    [Fact]
    public async Task Middleware_WithReferentialIntegrityException_ProducesCorrectJsonResponse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var referentialException = new ReferentialIntegrityException("Foreign key constraint violation");

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw referentialException,
            logger: mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        // Verify the JSON structure matches our expected format
        Assert.Contains("\"code\":\"REFERENTIAL_INTEGRITY_ERROR\"", responseBody);
        Assert.Contains("Foreign key constraint violation", responseBody);
    }

    [Fact]
    public async Task Middleware_WithGenericException_ProducesCorrectJsonResponse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var genericException = new Exception("Unexpected database error");

        var middleware = new GlobalExceptionMiddleware(
            next: (innerHttpContext) => throw genericException,
            logger: mockLogger.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        
        // Verify the JSON structure matches our expected format
        Assert.Contains("\"code\":\"INTERNAL_SERVER_ERROR\"", responseBody);
        Assert.Contains("An unexpected error occurred", responseBody);
    }
}