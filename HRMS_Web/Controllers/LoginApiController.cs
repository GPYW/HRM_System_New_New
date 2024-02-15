using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginApiController : ControllerBase
    {
        private readonly ILogger<LoginApiController> _logger;

        public LoginApiController(ILogger<LoginApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("logClick")]
        [AllowAnonymous]
        public IActionResult LogClick()
        {
            // Log the click or perform any other action you want here
            _logger.LogInformation("Login page click logged.");

            // Return a response indicating success or any other information
            return Ok(new { Message = "Click logged successfully" });
        }
    }
}
