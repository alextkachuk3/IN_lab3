using IN_lab3.Models;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IN_lab3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        private readonly SymmetricSecurityKey securityKey;
        private readonly SigningCredentials credentials;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
            securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key"));
            credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        [HttpPost("Signup")]
        public IActionResult Signup(string? username, string? password)
        {
            string? check_credentials = CheckCredentials(username, password);

            if (check_credentials is not null)
            {
                return BadRequest(check_credentials);
            }

            if (_userService.IsUserNameUsed(username!))
            {
                return BadRequest("Username already used!");
            }

            try
            {
                _userService.AddUser(username!, password!);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }

            return Ok();
        }

        [HttpPost("Login")]
        public IActionResult Login(UserDto userDto)
        {
            var check_credentials = CheckCredentials(userDto.Username, userDto.Password);

            if (check_credentials is not null)
            {
                return BadRequest(check_credentials);
            }

            User? user = _userService.GetUser(userDto.Username!);          

            if (user == null)
            {
                return BadRequest("User with this username not exists!");
            }
            else if (user.CheckCredentials(userDto.Password!, user.Salt!))
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username!) };                

                var token = new JwtSecurityToken(
                    issuer: "issuer",
                    audience: "audience",
                    claims: claims,
                    expires: DateTime.Now.AddYears(1),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { Token = tokenString });
            }
            else
            {
                return BadRequest("Wrong username or password!");
            }
        }

        private string? CheckCredentials(string? username, string? password)
        {
            if (username is null)
            {
                return "Username is empty";
            }

            if (password is null)
            {
                return "Password is empty";
            }

            if (username.Length > 30)
            {
                return "Length of username is bigger than max!";
            }

            if (username.Length < 5)
            {
                return "Minimal username lenght is 5!";
            }

            if (!Models.User.IsAlphanumeric(username))
            {
                return "Username contains special chars!";
            }

            if (!Models.User.IsAlphanumeric(password))
            {
                return "Username contains special chars!";
            }

            return null;
        }
    }
}
