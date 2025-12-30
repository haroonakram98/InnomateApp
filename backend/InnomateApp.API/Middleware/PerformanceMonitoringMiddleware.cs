using System.Diagnostics;

namespace InnomateApp.API.Middleware
{
    /// <summary>
    /// Middleware to track request performance metrics
    /// </summary>
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

        public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                // Log performance metrics
                _logger.LogInformation(
                    "Performance: {Endpoint} completed in {ElapsedMs}ms - Status: {StatusCode}",
                    endpoint, elapsedMs, context.Response.StatusCode);

                // Warn on slow requests (>1 second)
                if (elapsedMs > 1000)
                {
                    _logger.LogWarning(
                        "Slow request detected: {Endpoint} took {ElapsedMs}ms",
                        endpoint, elapsedMs);
                }

                // Track metrics for Prometheus (if needed later)
                // Metrics.Measure.Timer.Time(new TimerOptions { Name = "http_request_duration" }, elapsedMs);
            }
        }
    }
}
