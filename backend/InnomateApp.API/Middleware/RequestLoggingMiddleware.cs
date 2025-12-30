using Serilog.Context;

namespace InnomateApp.API.Middleware
{
    /// <summary>
    /// Middleware to add correlation ID to all requests for distributed tracing
    /// </summary>
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
            // Generate or retrieve correlation ID
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                ?? Guid.NewGuid().ToString();

            // Add to response headers for client tracking
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            // Push to Serilog context (will be included in all logs)
            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            {
                _logger.LogInformation("Request started: {Method} {Path}", 
                    context.Request.Method, context.Request.Path);

                try
                {
                    await _next(context);

                    _logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode}", 
                        context.Request.Method, context.Request.Path, context.Response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Request failed: {Method} {Path}", 
                        context.Request.Method, context.Request.Path);
                    throw;
                }
            }
        }
    }
}
