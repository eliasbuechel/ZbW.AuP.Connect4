using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.communication.Controller
{
    [Authorize]
    [ApiController]
    [Route("account")]
    public class AuthController : ControllerBase
    {
        [HttpPost("checkAuthentication")]
        public IActionResult CheckAuthentication()
        {
            var token = Request.Cookies["authToken"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return Ok(new { authenticated = true });
        }
    }
}
