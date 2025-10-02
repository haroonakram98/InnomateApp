using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Logging;

namespace InnomateApp.API.Middleware;

public sealed class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IAuditSink auditSink)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();

            var user = GetUserName(context.User);
            var action = $"{context.Request.Method} {context.Request.Path}";
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var status = context.Response.StatusCode;

            // enqueue — non-blocking
            auditSink.Enqueue(new AuditEntry(
                Action: action,
                PerformedBy: user,
                StatusCode: status,
                ElapsedMs: sw.ElapsedMilliseconds,
                Ip: ip,
                Details: context.Request.QueryString.HasValue ? $"Query={context.Request.QueryString.Value}" : null,
                OccurredAtUtc: DateTime.UtcNow
            ));
        }
    }

    private static string GetUserName(ClaimsPrincipal user) =>
    user?.Identity?.IsAuthenticated == true
        ? (user.FindFirst(ClaimTypes.Name)?.Value
           ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? user.FindFirst("sub")?.Value
           ?? "Unknown")
        : "Anonymous";
}
