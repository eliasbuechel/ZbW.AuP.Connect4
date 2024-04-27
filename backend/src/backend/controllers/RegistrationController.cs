using backend.Data;
using backend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace backend.controllers
{
    [ApiController]
    [Route("[controller]")]
    internal class RegistrationController : ControllerBase
    {
        public RegistrationController(UserManager<PlayerIdentity> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("SendVerificationEmail")]
        public async Task<IActionResult> SendVerificationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new { message = "Email is already confirmed." });
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "EmailVerification",
                new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok(new { message = "Verification email sent. Please check your email." });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest(new { message = "User ID and code are required" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = $"Unable to load user with ID '{userId}'." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email successfully confirmed." });
            }
            return BadRequest(new { message = "Error confirming email." });
        }

        private readonly IEmailSender _emailSender;
        private readonly UserManager<PlayerIdentity> _userManager;
    }
}
