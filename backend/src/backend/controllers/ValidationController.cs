using backend.database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.controllers
{
    [ApiController]
    [Route("[controller]")]
    internal class RegistrationController : ControllerBase
    {

        public RegistrationController(UserManager<PlayerIdentity> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("email-taken")]
        public IActionResult EmailTaken(string email)
        {
            if (_userManager.FindByEmailAsync(email).Result != null)
                return BadRequest("Email already registered.");

            return Ok();
        }

        private readonly UserManager<PlayerIdentity> _userManager;
    }
}
