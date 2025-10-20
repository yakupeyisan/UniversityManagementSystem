using UniversityMS.Domain.Exceptions;
using InvalidOperationException = UniversityMS.Domain.Exceptions.InvalidOperationException;
using UnauthorizedAccessException = UniversityMS.Domain.Exceptions.UnauthorizedAccessException;

namespace UniversityMS.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        return exception switch
        {
            ValidationException ve => HandleValidationException(context, ve, response),
            NotFoundException nfe => HandleNotFoundException(context, nfe, response),
            UnauthorizedAccessException uae => HandleUnauthorizedException(context, uae, response),
            ForbiddenException fe => HandleForbiddenException(context, fe, response),
            DuplicateException de => HandleDuplicateException(context, de, response),
            BusinessRuleException bre => HandleBusinessRuleException(context, bre, response),
            InvalidOperationException ioe => HandleInvalidOperationException(context, ioe, response),
            _ => HandleGenericException(context, exception, response)
        };
    }

    private static Task HandleValidationException(HttpContext context,
        ValidationException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.Message = ex.Message;
        response.ErrorCode = "VALIDATION_ERROR";
        response.Errors = ex.Errors;

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleNotFoundException(HttpContext context,
        NotFoundException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        response.Message = ex.Message;
        response.ErrorCode = "NOT_FOUND";

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleUnauthorizedException(HttpContext context,
        UnauthorizedAccessException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        response.Message = ex.Message;
        response.ErrorCode = "UNAUTHORIZED";

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleForbiddenException(HttpContext context,
        ForbiddenException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        response.Message = ex.Message;
        response.ErrorCode = "FORBIDDEN";

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleDuplicateException(HttpContext context,
        DuplicateException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        response.Message = ex.Message;
        response.ErrorCode = "DUPLICATE";

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleBusinessRuleException(HttpContext context,
        BusinessRuleException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        response.Message = ex.Message;
        response.ErrorCode = "BUSINESS_RULE_VIOLATION";

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleInvalidOperationException(HttpContext context,
        InvalidOperationException ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.Message = ex.Message;
        response.ErrorCode = "INVALID_OPERATION";

        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleGenericException(HttpContext context,
        Exception ex, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        response.Message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
        response.ErrorCode = "INTERNAL_ERROR";

        return context.Response.WriteAsJsonAsync(response);
    }
}