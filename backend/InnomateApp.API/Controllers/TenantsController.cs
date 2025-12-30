using System.Threading.Tasks;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("onboard")]
        [AllowAnonymous] // Allowing public signup for now
        public async Task<IActionResult> Onboard(TenantOnboardingDto dto)
        {
            try
            {
                var result = await _tenantService.OnboardTenantAsync(dto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tenantService.GetAllTenantsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _tenantService.GetTenantByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
