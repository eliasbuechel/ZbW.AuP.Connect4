using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.communication.controller
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(SignInManager<PlayerIdentity> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("checkAuthentication")]
        [Authorize]
        public IActionResult CheckAuthentication()
        {
            return Ok(new { isAuthenticated = true });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully"});
        }

        private readonly SignInManager<PlayerIdentity> _signInManager;
    }
}
