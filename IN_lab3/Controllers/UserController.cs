using IN_lab3.Models;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult Signup(UserDto userDto)
        {
            string? check_credentials = CheckCredentials(userDto.Username, userDto.Password);

            if (check_credentials is not null)
            {
                return BadRequest(new { Error = check_credentials });
            }

            if (_userService.IsUserNameUsed(userDto.Username!))
            {
                return BadRequest(new { Error = "username_already_used" });
            }

            try
            {
                _userService.AddUser(userDto.Username, userDto.Password);
            }
            catch
            {
                return StatusCode(500, new { Error = "internal_server_error" });
            }

            return Login(userDto);
        }

        [HttpPost("Login")]
        public IActionResult Login(UserDto userDto)
        {
            var check_credentials = CheckCredentials(userDto.Username, userDto.Password);

            if (check_credentials is not null)
            {
                return BadRequest(new { Error = check_credentials });
            }

            User? user = _userService.GetUser(userDto.Username!);

            if (user == null)
            {
                return BadRequest(new { Error = "wrong_username_or_password" });
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
                return BadRequest(new { Error = "wrong_username_or_password" });
            }
        }

        [Authorize]
        [HttpGet("Username")]
        public IActionResult GetUsername()
        {
            return Ok(new { _userService.GetUser(User.Identity!.Name!)!.Username });
        }

        private string? CheckCredentials(string? username, string? password)
        {
            if (username is null)
            {
                return "username_is_empty";
            }

            if (password is null)
            {
                return "password_is_empty";
            }

            if (username.Length > 30)
            {
                return "username_length_is_bigger_than_30";
            }

            if (username.Length < 5)
            {
                return "username_lenght_is_smaller_than_5";
            }

            if (!Models.User.IsAlphanumeric(username))
            {
                return "username_contains_special_chars";
            }

            if (!Models.User.IsAlphanumeric(password))
            {
                return "password_contains_special_chars";
            }

            return null;
        }
    }
}
