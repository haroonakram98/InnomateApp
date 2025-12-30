using System.Collections.Generic;
using System.Threading.Tasks;
using InnomateApp.Application.DTOs;

namespace InnomateApp.Application.Interfaces
{
    public interface ITenantService
    {
        Task<TenantResponseDto> OnboardTenantAsync(TenantOnboardingDto dto);
        Task<IEnumerable<TenantResponseDto>> GetAllTenantsAsync();
        Task<TenantResponseDto> GetTenantByIdAsync(int id);
    }
}
