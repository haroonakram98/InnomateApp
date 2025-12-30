using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public TenantService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<TenantResponseDto> OnboardTenantAsync(TenantOnboardingDto dto)
        {
            // 1. Check if tenant code exists
            var existingTenant = await _unitOfWork.Tenants.GetByCodeAsync(dto.TenantCode);
            if (existingTenant != null)
            {
                throw new System.Exception($"Tenant with code '{dto.TenantCode}' already exists.");
            }

            // 2. Start transaction
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 3. Create Tenant
                var tenant = Tenant.Create(dto.TenantName, dto.TenantCode);
                await _unitOfWork.Tenants.AddAsync(tenant);
                await _unitOfWork.SaveChangesAsync(); // Get TenantId

                // 4. Create Admin User for this Tenant
                var hashedPassword = _passwordHasher.HashPassword(dto.AdminPassword);
                var adminUser = User.Create(tenant.TenantId, dto.AdminUsername, dto.AdminEmail, hashedPassword);
                
                // 5. Assign "Admin" role
                var adminRole = await _unitOfWork.Roles.GetByNameAsync("Admin");
                if (adminRole != null)
                {
                    adminUser.Roles = new List<Role> { adminRole };
                }

                await _unitOfWork.Users.CreateUserAsync(adminUser);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return new TenantResponseDto
                {
                    TenantId = tenant.TenantId,
                    Name = tenant.Name,
                    Code = tenant.Code,
                    AdminUsername = adminUser.Username
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<TenantResponseDto>> GetAllTenantsAsync()
        {
            var tenants = await _unitOfWork.Tenants.GetAllAsync();
            return tenants.Select(t => new TenantResponseDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code
            });
        }

        public async Task<TenantResponseDto> GetTenantByIdAsync(int id)
        {
            var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);
            if (tenant == null) return null!;

            return new TenantResponseDto
            {
                TenantId = tenant.TenantId,
                Name = tenant.Name,
                Code = tenant.Code
            };
        }
    }
}
