using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Logging;
using InnomateApp.Domain.Entities;               // ← your AuditLog entity
using InnomateApp.Infrastructure.Persistence;     // ← AppDbContext
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InnomateApp.Infrastructure.Logging;

public sealed class AuditSink : BackgroundService, IAuditSink
{
    private readonly Channel<AuditEntry> _channel;
    private readonly IServiceProvider _services;
    private readonly ILogger<AuditSink> _logger;

    // tune these to your traffic profile
    private const int BatchSize = 50;
    private const int MaxDelayMs = 1000;

    public AuditSink(IServiceProvider services, ILogger<AuditSink> logger)
    {
        _services = services;
        _logger = logger;

        _channel = Channel.CreateBounded<AuditEntry>(
            new BoundedChannelOptions(capacity: 5000)
            {
                FullMode = BoundedChannelFullMode.DropOldest // backpressure to protect memory
            });
    }

    public void Enqueue(AuditEntry entry)
    {
        // Non-throwing enqueue; if buffer is full, oldest will be dropped (above policy)
        _channel.Writer.TryWrite(entry);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = _channel.Reader;
        var buffer = new List<AuditEntry>(BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                buffer.Clear();

                // block for first item
                var first = await reader.ReadAsync(stoppingToken);
                buffer.Add(first);

                // coalesce a batch (or until delay elapses)
                var delay = Task.Delay(MaxDelayMs, stoppingToken);
                while (buffer.Count < BatchSize && reader.TryRead(out var more))
                {
                    buffer.Add(more);
                }
                // give a little more time for batch fill
                await Task.WhenAny(delay);

                // persist batch
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (var e in buffer)
                {
                    db.AuditLogs.Add(new AuditLog
                    {
                        Action = e.Action,
                        PerformedBy = e.PerformedBy,
                        PerformedAt = e.OccurredAtUtc,
                        // pack extra metrics into Details (or add real columns later)
                        Details = $"Status={e.StatusCode};ElapsedMs={e.ElapsedMs};IP={e.Ip};{e.Details}"
                    });
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist audit batch");
                // brief backoff to avoid tight error loop
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
