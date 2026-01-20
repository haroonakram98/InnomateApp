using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace InnomateApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }

        [HttpGet("ready")]
        public async Task<IActionResult> GetReady()
        {
            try
            {
                // This endpoint can be used by orchestration tools (Kubernetes, etc.)
                // to determine if the application is ready to serve traffic
                
                return Ok(new
                {
                    Status = "Ready",
                    Timestamp = DateTime.UtcNow,
                    Database = "Seeding in progress...",
                    Message = "API is ready to serve requests"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        [HttpGet("live")]
        public IActionResult GetLive()
        {
            // This endpoint indicates if the application is alive
            // It should return 200 as long as the application process is running
            return Ok(new
            {
                Status = "Alive",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
