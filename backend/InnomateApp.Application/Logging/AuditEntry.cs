namespace InnomateApp.Application.Logging;

public sealed record AuditEntry(
    string Action,
    string PerformedBy,
    int StatusCode,
    long ElapsedMs,
    string? Ip,
    string? Details,
    DateTime OccurredAtUtc
);