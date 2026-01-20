using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Application.Services;
using InnomateApp.Infrastructure.BackgroundServices;
using InnomateApp.Infrastructure.Behaviors;
using InnomateApp.Infrastructure.Data;
using InnomateApp.Infrastructure.Identity;
using InnomateApp.Infrastructure.Logging;
using InnomateApp.Infrastructure.Persistence;
using InnomateApp.Infrastructure.Repositories;
using InnomateApp.Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InnomateApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IStockSummaryRepository, StockSummaryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<ISaleDetailRepository, SaleDetailRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IReturnRepository, ReturnRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantProvider, HttpTenantProvider>();
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Security
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Logging & Audit
            services.AddSingleton<AuditSink>();
            services.AddSingleton<IAuditSink>(sp => sp.GetRequiredService<AuditSink>());
            services.AddHostedService(sp => sp.GetRequiredService<AuditSink>());
            services.AddSingleton<ILoginUpdateQueue, InMemoryLoginUpdateQueue>();
            services.AddHostedService<LoginUpdateWorker>();

            // Transaction Behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EnhancedTransactionBehavior<,>));

            // Database Seeding & Health Check
            services.AddHostedService<DatabaseSeedingService>();
            services.AddHostedService<DatabaseHealthCheckService>();

            return services;
        }
    }
}
