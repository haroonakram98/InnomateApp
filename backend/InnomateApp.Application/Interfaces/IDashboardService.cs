using InnomateApp.Application.DTOs;
using System.Threading.Tasks;

namespace InnomateApp.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetDashboardStatsAsync();
    }
}
