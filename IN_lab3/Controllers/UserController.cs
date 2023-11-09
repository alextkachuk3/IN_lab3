using IN_lab3.Models;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IN_lab3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("Signup")]
        public IActionResult Signup(string? username, string? password)
        {
            string? check_credentials = CheckCredentials(username, password);

            if (check_credentials is not null)
            {
                return BadRequest(check_credentials);
            }

            if(_userService.IsUserNameUsed(username!))
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
        public IActionResult Login(string username, string password)
        {
            User? user = _userService.GetUser(username);

            var check_credentials = CheckCredentials(username, password);

            if (check_credentials is not null)
            {
                return BadRequest(check_credentials);
            }

            if (user == null)
            {
                return BadRequest("User with this username not exists!");               
            }
            else if (user.CheckCredentials(password, user.Salt!))
            {
                return Ok(user.Id);
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
