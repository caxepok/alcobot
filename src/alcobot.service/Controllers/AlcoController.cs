using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace alcobot.service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlcoController : ControllerBase
    {
        private readonly ILogger<AlcoController> _logger;

        public AlcoController(ILogger<AlcoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
