using IN_lab3.Models;
using IN_lab3.Services.MusicService;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace IN_lab3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IMusicService _musicService;

        public MusicController(ILogger<UserController> logger, IUserService userService, IMusicService musicService)
        {
            _logger = logger;
            _userService = userService;
            _musicService = musicService;
        }


        [Authorize]
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file, string name)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Invalid file");
            }

            var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Ok("File uploaded successfully");
        }
    }
}
