using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InnomateApp.Infrastructure.Data;
using System.Net;

namespace InnomateApp.Infrastructure.Persistence
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync(CancellationToken ct = default);
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            int retryCount = 0;
            const int maxRetries = 10; // Increased for slower first-time startups
            var delay = TimeSpan.FromSeconds(3); // Increased initial wait

            while (retryCount < maxRetries)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var connectionString = context.Database.GetConnectionString();
                    var serverName = connectionString?.Split(';').FirstOrDefault(x => x.StartsWith("Server=", StringComparison.OrdinalIgnoreCase)) ?? "Unknown";

                    _logger.LogInformation("🚀 API is warming up... Attempting to connect to: {Server} (Attempt {Count}/{Max})", 
                        serverName, retryCount + 1, maxRetries);

                    // --- ADVANCED DIAGNOSTIC: DNS resolution ---
                    try
                    {
                        var rawServer = serverName.Replace("Server=", "", StringComparison.OrdinalIgnoreCase).Trim();
                        // Handle cases like "sqlserver,1433"
                        var hostname = rawServer.Split(',')[0].Split('\\')[0].Trim(); 
                        
                        _logger.LogInformation("🔍 Diagnostic: Attempting to resolve hostname '{Hostname}' (from '{Raw}')", hostname, rawServer);
                        
                        var addresses = await Dns.GetHostAddressesAsync(hostname, ct);
                        _logger.LogInformation("✅ DNS Success: '{Hostname}' -> {IPs}", hostname, string.Join(", ", addresses.Select(a => a.ToString())));
                    }
                    catch (Exception dnsEx)
                    {
                        _logger.LogWarning("❌ DNS Failure: Could not resolve the server address. Details: {Message}", dnsEx.Message);
                        
                        // Check if ANY DNS works
                        try {
                            var check = await Dns.GetHostAddressesAsync("google.com", ct);
                            _logger.LogInformation("🌐 Internet DNS Check: 'google.com' is reachable. The issue is likely internal to Docker networking.");
                        } catch {
                            _logger.LogCritical("🚫 Network Isolation: This container cannot resolve ANY hostnames (even google.com). Check your Firewall/VPN.");
                        }
                    }
                    // -------------------------------------------

                    // Ensure database is created and migrations are applied
                    await context.Database.MigrateAsync(ct);

                    // Seed data
                    SeedData.Seed(scope.ServiceProvider);

                    _logger.LogInformation("✅ Database initialization successfully completed!");
                    return;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning("⏳ SQL Server is still starting up or unreachable. Retrying in {Seconds}s... (Error: {Message})", 
                        delay.TotalSeconds, ex.Message);

                    if (retryCount >= maxRetries)
                    {
                        _logger.LogCritical("❌ Database initialization failed after {Max} retries. The API will stay running for inspection.", maxRetries);
                        throw; 
                    }

                    await Task.Delay(delay, ct);
                    delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 1.5, 30)); // Slightly slower backoff
                }
            }
        }
    }
}
