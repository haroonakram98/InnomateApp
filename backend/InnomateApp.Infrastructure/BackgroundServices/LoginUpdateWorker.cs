using InnomateApp.Application.Interfaces;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class LoginUpdateWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoginUpdateQueue _queue;
        private readonly ILogger<LoginUpdateWorker> _logger;

        public LoginUpdateWorker(IServiceProvider serviceProvider, ILoginUpdateQueue queue, ILogger<LoginUpdateWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[LoginUpdateWorker] Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var userId))
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var user = await db.Users.FindAsync(userId);
                        if (user != null)
                        {
                            user.LastLoginAt = DateTime.Now;
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                    else
                    {
                        await Task.Delay(250, stoppingToken); // reduce busy loop
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[LoginUpdateWorker] Error updating last login");
                }
            }

            _logger.LogInformation("[LoginUpdateWorker] Stopped");
        }
    }
}
