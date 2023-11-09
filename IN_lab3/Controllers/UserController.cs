using IN_lab3.Models;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("Registration")]
        public IActionResult Registration(string? username, string? password)
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
