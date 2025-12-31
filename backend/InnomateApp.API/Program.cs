using AutoMapper;
using FluentValidation;
using InnomateApp.Application.Common.Validators;
using InnomateApp.Application.Common.Behavior;
using InnomateApp.API.Middleware;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.IServices;
using InnomateApp.Application.Interfaces.Repositories;
using InnomateApp.Application.Interfaces.Services;
using InnomateApp.Application.Mappings;
using InnomateApp.Application.Services;
using InnomateApp.Infrastructure.Logging;
using InnomateApp.Infrastructure.Persistence;
using InnomateApp.Infrastructure.Repositories;
using InnomateApp.Infrastructure.Data;
using InnomateApp.Infrastructure.Security;
using InnomateApp.API.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Serilog;
using Serilog.Context;
using MediatR;

// ✅ Phase 0: Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "InnomateApp")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/innomate-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting InnomateApp API");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI Services
// Application layer services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IFifoService, FifoService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IReturnService, ReturnService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ISequenceService, SequenceService>();
// Infrastructure repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IStockSummaryRepository, StockSummaryRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISaleDetailRepository, SaleDetailRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IReturnRepository, ReturnRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
// audit sink as background service + interface
builder.Services.AddSingleton<AuditSink>();
builder.Services.AddSingleton<IAuditSink>(sp => sp.GetRequiredService<AuditSink>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<AuditSink>());

builder.Services.AddSingleton<ILoginUpdateQueue, InMemoryLoginUpdateQueue>();
builder.Services.AddHostedService<LoginUpdateWorker>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// JWT Config
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = key
        };
    });



// Authorization MUST BE BEFORE .Build()
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreatePurchaseDtoValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    SeedData.Seed(scope.ServiceProvider);
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

// ✅ Phase 0: Observability Middleware (must be early in pipeline)
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<PerformanceMonitoringMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditMiddleware>();

app.MapControllers();


Log.Information("InnomateApp API started successfully");
app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
