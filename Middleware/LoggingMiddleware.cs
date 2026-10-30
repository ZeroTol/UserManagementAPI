using System.Diagnostics;

namespace UserManagementAPI.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("Incoming request: {Method} {Path}", method, path);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Outgoing response: {Method} {Path} -> {StatusCode} ({Elapsed} ms)",
                method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}