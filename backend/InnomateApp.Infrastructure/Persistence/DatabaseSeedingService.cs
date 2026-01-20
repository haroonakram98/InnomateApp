using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InnomateApp.Domain.Entities;
using InnomateApp.Infrastructure.Security;
using InnomateApp.Application.Interfaces;

namespace InnomateApp.Infrastructure.Persistence
{
    public class DatabaseSeedingService : BackgroundService
    {
        private readonly ILogger<DatabaseSeedingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _retryInterval = TimeSpan.FromSeconds(30);
        private readonly int _maxRetries = 10;

        public DatabaseSeedingService(
            ILogger<DatabaseSeedingService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Database seeding service started");

            // Wait for database to be fully ready
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            var retryCount = 0;
            while (!stoppingToken.IsCancellationRequested && retryCount < _maxRetries)
            {
                try
                {
                    await SeedDatabaseAsync(stoppingToken);
                    _logger.LogInformation("Database seeding completed successfully");
                    break; // Success, exit the retry loop
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, 
                        "Database seeding attempt {RetryCount}/{MaxRetries} failed. " +
                        "Retrying in {RetryInterval} seconds", 
                        retryCount, _maxRetries, _retryInterval.TotalSeconds);

                    if (retryCount >= _maxRetries)
                    {
                        _logger.LogError("Database seeding failed after {MaxRetries} attempts. " +
                                       "Application will continue running but database may not be properly seeded.", 
                                       _maxRetries);
                        break;
                    }

                    await Task.Delay(_retryInterval, stoppingToken);
                }
            }

            if (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Database seeding service was cancelled");
            }
        }

        private async Task SeedDatabaseAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            // Check if database is accessible
            if (!await CanConnectToDatabaseAsync(context, cancellationToken))
            {
                throw new InvalidOperationException("Cannot connect to database");
            }

            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync(cancellationToken);

            // Seed Roles first
            await SeedRolesAsync(context, cancellationToken);
            
            // Save roles to database before using them
            await context.SaveChangesAsync(cancellationToken);
            
            // Seed Admin User
            await SeedAdminUserAsync(context, passwordHasher, cancellationToken);

            // Save admin user
            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> CanConnectToDatabaseAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            try
            {
                return await context.Database.CanConnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Database connection check failed");
                return false;
            }
        }

        private async Task SeedRolesAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            if (!await context.Roles.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Seeding roles");
                
                var roles = new[]
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" },
                    new Role { Name = "Manager" }
                };

                await context.Roles.AddRangeAsync(roles, cancellationToken);
                _logger.LogInformation("Seeded {Count} roles", roles.Length);
            }
            else
            {
                _logger.LogDebug("Roles already exist, skipping seeding");
            }
        }

        private async Task SeedAdminUserAsync(AppDbContext context, IPasswordHasher passwordHasher, CancellationToken cancellationToken)
        {
            if (!await context.Users.IgnoreQueryFilters().AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Seeding admin user");

                var adminUser = User.Create(1, "admin", "admin@innomate.com", passwordHasher.HashPassword("Admin@123!"));


                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin", cancellationToken);
                adminUser.Roles = new List<Role> { adminRole };

                await context.Users.AddAsync(adminUser, cancellationToken);
                _logger.LogInformation("Seeded admin user with ID: {UserId}", adminUser.UserId);
            }
            else
            {
                _logger.LogDebug("Admin user already exists, skipping seeding");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Database seeding service is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}
