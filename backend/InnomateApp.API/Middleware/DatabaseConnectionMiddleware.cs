using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InnomateApp.API.Middleware
{
    public class DatabaseConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseConnectionMiddleware> _logger;

        public DatabaseConnectionMiddleware(RequestDelegate next, ILogger<DatabaseConnectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip database connection check for health endpoints and static files
            if (context.Request.Path.StartsWithSegments("/api/health") || 
                context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/.well-known"))
            {
                await _next(context);
                return;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex) when (IsDatabaseConnectionException(ex))
            {
                _logger.LogWarning(ex, "Database connection issue detected for request {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
                
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Service temporarily unavailable",
                    message = "Database connection is being established. Please try again in a moment.",
                    timestamp = DateTime.UtcNow,
                    requestId = context.TraceIdentifier
                });
            }
        }

        private static bool IsDatabaseConnectionException(Exception ex)
        {
            // Check for common database connection exceptions
            return ex.Message.Contains("database") || 
                   ex.Message.Contains("connection") ||
                   ex.Message.Contains("timeout") ||
                   ex.Message.Contains("unavailable") ||
                   ex.InnerException?.Message.Contains("database") == true ||
                   ex.InnerException?.Message.Contains("connection") == true;
        }
    }
}
