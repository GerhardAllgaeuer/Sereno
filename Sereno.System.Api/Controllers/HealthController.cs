using Microsoft.AspNetCore.Mvc;

namespace Connexia.System.Api.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("api/v1/[controller]")]
        public IActionResult Get()
        {
            return Ok(new { Status = "Healthy" });
        }
    }
} 