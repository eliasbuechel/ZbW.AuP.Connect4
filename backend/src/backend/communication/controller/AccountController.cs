using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.communication.controller
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet("checkAuthentication")]
        [Authorize]
        public IActionResult CheckAuthentication()
        {
            return Ok(new { isAuthenticated = true });
        }
    }
}
