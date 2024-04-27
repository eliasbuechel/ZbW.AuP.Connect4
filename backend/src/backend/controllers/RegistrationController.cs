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



        private readonly UserManager<PlayerIdentity> _userManager;
    }
}
