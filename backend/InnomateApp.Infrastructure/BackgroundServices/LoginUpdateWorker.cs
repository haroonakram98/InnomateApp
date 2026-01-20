using InnomateApp.Application.Interfaces;
using InnomateApp.Infrastructure.Identity;
using InnomateApp.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InnomateApp.Infrastructure.BackgroundServices
{
    public class LoginUpdateWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LoginUpdateWorker> _logger;

        public LoginUpdateWorker(IServiceProvider serviceProvider, ILogger<LoginUpdateWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[LoginUpdateWorker] Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var queue = scope.ServiceProvider.GetRequiredService<ILoginUpdateQueue>();

                    if (queue.TryDequeue(out var userId))
                    {
                        // Get the tenant ID for this user
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                        var user = await userRepository.GetUserByIdIgnoreFilterAsync(userId);

                        if (user == null)
                        {
                            _logger.LogWarning("[LoginUpdateWorker] User {UserId} not found", userId);
                            continue;
                        }

                        // Create a scoped context with the user's tenant ID
                        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
                        if (tenantProvider is HttpTenantProvider httpTenantProvider)
                        {
                            // Set the tenant ID for this operation
                            httpTenantProvider.SetTenantIdForBackgroundOperation(user.TenantId);
                        }

                        try
                        {
                            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                            var dbUser = await db.Users.FindAsync(new object[] { userId }, cancellationToken: stoppingToken);

                            if (dbUser != null)
                            {
                                dbUser.LastLoginAt = DateTime.Now;
                                await db.SaveChangesAsync(stoppingToken);
                                _logger.LogInformation("[LoginUpdateWorker] Updated last login for user {UserId}", userId);
                            }
                        }
                        finally
                        {
                            // Reset the tenant ID
                            if (tenantProvider is HttpTenantProvider httpTenantProviderReset)
                            {
                                httpTenantProviderReset.ResetTenantId();
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(250, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[LoginUpdateWorker] Error updating last login");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("[LoginUpdateWorker] Stopped");
        }
    }


}