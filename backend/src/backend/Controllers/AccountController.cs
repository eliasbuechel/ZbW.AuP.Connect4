//using backend.Data;
//using backend.ViewModels;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using System.Text.Encodings.Web;

//namespace backend.controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class AccountController : ControllerBase
//    {
//        public AccountController(UserManager<PlayerIdentity> userManager, IEmailSender emailSender)
//        {
//            _userManager = userManager;
//            _emailSender = emailSender;
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register(RegisterDTO model)
//        {
//            var user = new PlayerIdentity { UserName = model.Email, Email = model.Email };
//            var result = await _userManager.CreateAsync(user, model.Password);

//            if (result.Succeeded)
//            {
//                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, Request.Scheme);

//                await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
//                    $"Please confirm your account by clicking here: <a href='{callbackUrl}'>link</a>");

//                return Ok();
//            }

//            return BadRequest(result.Errors);
//        }

//        [HttpGet("confirmEmail")]
//        public async Task<IActionResult> ConfirmEmail(string userId, string code)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null) return BadRequest("Unable to find user.");

//            var result = await _userManager.ConfirmEmailAsync(user, code);
//            if (result.Succeeded)
//            {
//                return Ok("Email confirmed successfully.");
//            }
//            return BadRequest("Error confirming your email.");
//        }

//        private readonly IEmailSender _emailSender;
//        private readonly UserManager<PlayerIdentity> _userManager;
//    }
//}
