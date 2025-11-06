using Microsoft.AspNetCore.Mvc;

namespace InnomateApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Hello from InnomateApp API .....f🚀");
}