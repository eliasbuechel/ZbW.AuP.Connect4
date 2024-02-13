using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Connect4Api
{
    public class Connect4Hub : Hub
    {
        public Connect4Hub(LoginService loginService)
        {
            _loginService = loginService;
        }
        public async Task Register(string username, string email, string password)
        {
            try
            {
                string unverifyedUserJwt = _loginService.Register(username, email, password);
                await Clients.Caller.SendAsync("registration-successful", unverifyedUserJwt);
            }
            catch (CridentialsNotValidException ex)
            {
                await Clients.Caller.SendAsync("registration-failed", ex.Message);
            }
        }
        public async Task Login(string email, string password)
        {
            try
            {
                string loggedInJwt = _loginService.Login(email, password);
                await Clients.Caller.SendAsync("login-successful", loggedInJwt);
                // notify other users about loggin
            }
            catch (EmailNotVerifyedException ex)
            {
                await Clients.Caller.SendAsync("login-succesfull-with-open-email-verification", ex.UnverifiedEmailJwt);
            }
            catch (AllreadyLoggedInException ex)
            {
                await Clients.Caller.SendAsync("login-failed", ex.Message);
            }
            catch (CridentialsNotValidException ex)
            {
                await Clients.Caller.SendAsync("login-failed", ex.Message);
            }
        }
        public async Task CheckEmailVerificationCode(string unverifyedUserJwt, string verificationCode)
        {
            try
            {
                string loggedInJwt = _loginService.ValidateEmail(unverifyedUserJwt, verificationCode);
                await Clients.Caller.SendAsync("email-verification-successfull", loggedInJwt);
            }
            catch (InvalidJwtException)
            {
                await Clients.Caller.SendAsync("redirect-to-login");
            }
            catch (InvalidEmailVerificationCodeException ex)
            {
                await Clients.Caller.SendAsync("email-verification-failed", ex.Message);
            }
        }
        public async Task ResendEmailVerificationCode(string unverifyedUserJwt)
        {
            try
            {
                _loginService.ResendEmailVerificationCode(unverifyedUserJwt);
                await Clients.Caller.SendAsync("email-verification-code-resent");
            }
            catch (InvalidJwtException)
            {
                await Clients.Caller.SendAsync("redirect-to-login");
            }
        }
        public async Task Logout(string loggedInUserJwt)
        {
            try
            {
                _loginService.Logout(loggedInUserJwt);
                await Clients.Caller.SendAsync("logged-out");
            }
            catch (InvalidJwtException)
            {
                await Clients.Caller.SendAsync("redirect-to-login");
            }
            // notify other users about loggout
        }

        public override async Task OnConnectedAsync()
        {
            await Console.Out.WriteLineAsync("User " + Context.ConnectionId + " connected!");

            // maby handle reconnection...
        }
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            await Console.Out.WriteLineAsync("User " + Context.ConnectionId + " disconected!");

            //if (_loginService.IsLoggedIn(Context.ConnectionId))
            //{
            //    Guid userId = _loginService.LogoutCauseOfConnectionLoss(Context.ConnectionId);
            //    await Clients.Others.SendAsync("user-logged-out", userId);
            //}
        }

        private readonly LoginService _loginService;
    }

    public class LoginService
    {
        public LoginService(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public string Register(string username, string email, string password)
        {
            if (String.IsNullOrEmpty(username)) throw new CridentialsNotValidException("Username is invalid");
            if (String.IsNullOrEmpty(username)) throw new CridentialsNotValidException("Email is invalid");
            if (String.IsNullOrEmpty(email)) throw new CridentialsNotValidException("Password is invalid");

            foreach (User u in _users)
            {
                if (u.UserName == username) throw new CridentialsNotValidException("Username already used");
                if (u.Email == email) throw new CridentialsNotValidException("Email already registered");
            }

            User user = new User(username, email, password);
            _users.Add(user);

            // code to send verification code

            string unverifiedEmailUserJwt = _jwtService.GenerateToken(user.Id.ToString());
            _jwtUnverifyiedUserMappings.Add(unverifiedEmailUserJwt, user);
            return unverifiedEmailUserJwt;
        }
        public string Login(string email, string password)
        {
            foreach (User user in _users)
            {
                if (user.Email != email)
                    continue;

                if (user.Password != password)
                    throw new CridentialsNotValidException("Invalid password");

                foreach (var mapping in _jwtLoggedInUserMappings)
                    if (mapping.Value == user)
                        throw new AllreadyLoggedInException("Allready logged in with this account");
                    
                if (!user.IsEmailVerifyed)
                {
                    string unverifiedEmailUserJwt = _jwtService.GenerateToken(user.Id.ToString());
                    _jwtUnverifyiedUserMappings.Add(unverifiedEmailUserJwt, user);
                    throw new EmailNotVerifyedException(unverifiedEmailUserJwt);
                }

                string loggedInUserJwt = _jwtService.GenerateToken(user.Id.ToString());
                _jwtLoggedInUserMappings.Add(loggedInUserJwt, user);
                return loggedInUserJwt;
            }

            throw new CridentialsNotValidException("Email not registered");
        }
        public string ValidateEmail(string unverifyedUserJwt, string verificationCode)
        {
            if (!_jwtUnverifyiedUserMappings.ContainsKey(unverifyedUserJwt))
                throw new InvalidJwtException();

            User user = _jwtUnverifyiedUserMappings[unverifyedUserJwt];

            if (user.EmailVerificationCode != verificationCode)
                throw new InvalidEmailVerificationCodeException("Verification code not valid");

            _jwtUnverifyiedUserMappings.Remove(unverifyedUserJwt);
            string loggedInUserJwt = _jwtService.GenerateToken(user.Id.ToString());
            _jwtLoggedInUserMappings.Add(loggedInUserJwt, user);
            user.IsEmailVerifyed = true;

            return loggedInUserJwt;
        }
        public void Logout(string loggedInUserJwt)
        {
            if (!_jwtLoggedInUserMappings.ContainsKey(loggedInUserJwt))
                throw new InvalidJwtException();

            _jwtLoggedInUserMappings.Remove(loggedInUserJwt);
        }
        public void ResendEmailVerificationCode(string unverifyedUserJwt)
        {
            if (!_jwtUnverifyiedUserMappings.ContainsKey(unverifyedUserJwt))
                throw new InvalidJwtException();

            User user = _jwtUnverifyiedUserMappings[unverifyedUserJwt];

            // code to resend verification code
        }

        ICollection<User> _users = new List<User>();
        private readonly JwtService _jwtService;

        IDictionary<string, User> _jwtLoggedInUserMappings = new Dictionary<string, User>();
        IDictionary<string, User> _jwtUnverifyiedUserMappings = new Dictionary<string, User>();
    }


    public class User
    {
        public User(string userName, string email, string password)
        {
            _username = userName;
            _email = email;
            _password = password;

            _emailVerificationCode = "1234";
        }

        public string UserName => _username;
        public string Email => _email;
        public string Password => _password;
        public Guid Id => _id;
        public string EmailVerificationCode => _emailVerificationCode;
        public bool IsEmailVerifyed { get; set; }

        private readonly string _username;
        private readonly string _email;
        private readonly string _password;
        private readonly Guid _id = Guid.NewGuid();
        private readonly string _emailVerificationCode;
        private bool _isEmailVerifyed;
    }

    public class EmailNotVerifyedException : Exception
    {
        public EmailNotVerifyedException(string unverifiedEmailJwt)
        {
            UnverifiedEmailJwt = unverifiedEmailJwt;
        }
        public string UnverifiedEmailJwt { get; set; }
    }

    public class CridentialsNotValidException : Exception
    {
        public CridentialsNotValidException(string? message) : base(message)
        {
        }
    }

    public class InvalidEmailVerificationCodeException : Exception
    {
        public InvalidEmailVerificationCodeException(string? message) : base(message)
        {
        }
    }

    public class AllreadyLoggedInException : Exception
    {
        public AllreadyLoggedInException(string? message) : base(message)
        {
        }
    }

    public class InvalidJwtException() : Exception
    {

    }

    public class JwtService
    {
        private readonly string _secretKey;

        public JwtService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string GenerateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}