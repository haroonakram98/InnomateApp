using AutoMapper;
using FluentValidation;
using InnomateApp.Application.Common.Behavior;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.Services;
using InnomateApp.Application.Mappings;
using InnomateApp.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InnomateApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // MediatR
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreatePurchaseDtoValidator>();
            
            // Pipeline Behaviors (Order matters: Validation -> Transaction)
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Application Services
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            // Note: ProductService removed, replaced by MediatR Handlers
            // services.AddScoped<IProductService, ProductService>();
            // Note: Categories module uses MediatR handlers (CQRS pattern) - no service registration needed
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IFifoService, FifoService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            // services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IReturnService, ReturnService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ISequenceService, SequenceService>();

            return services;
        }
    }
}
