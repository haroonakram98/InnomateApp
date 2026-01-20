using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InnomateApp.Infrastructure.Persistence;

namespace InnomateApp.Infrastructure.Persistence
{
    public class DatabaseHealthCheckService : BackgroundService
    {
        private readonly ILogger<DatabaseHealthCheckService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public DatabaseHealthCheckService(
            ILogger<DatabaseHealthCheckService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Database health check service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckDatabaseHealthAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database health check failed");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Database health check service stopped");
        }

        private async Task CheckDatabaseHealthAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                var canConnect = await context.Database.CanConnectAsync(cancellationToken);
                
                if (canConnect)
                {
                    // Try a simple query to ensure database is responsive
                    await context.Roles.CountAsync(cancellationToken);
                    _logger.LogDebug("Database health check: Healthy");
                }
                else
                {
                    _logger.LogWarning("Database health check: Cannot connect to database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check: Database is not responding");
                throw;
            }
        }
    }
}
