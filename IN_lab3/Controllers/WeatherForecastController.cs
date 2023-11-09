using IN_lab3.Models;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IN_lab3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUserService _userService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [Authorize]
        [HttpGet(Name = "GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            return Enumerable.Range(1, 5).Select(index => new User(Summaries[Random.Shared.Next(Summaries.Length)], Summaries[Random.Shared.Next(Summaries.Length)])).ToArray();
        }

    }
}