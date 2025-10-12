namespace UniversityMS.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestTime = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        _logger.LogInformation("Request: {Method} {Path} started at {Time}",
            requestMethod, requestPath, requestTime);

        await _next(context);

        var responseTime = DateTime.UtcNow;
        var duration = (responseTime - requestTime).TotalMilliseconds;

        _logger.LogInformation("Request: {Method} {Path} completed in {Duration}ms with status {StatusCode}",
            requestMethod, requestPath, duration, context.Response.StatusCode);
    }
}