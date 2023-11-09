using IN_lab3.Models;
using IN_lab3.Services.MusicService;
using IN_lab3.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IN_lab3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IMusicService _musicService;
        private readonly string uploadsFolder;

        public MusicController(ILogger<UserController> logger, IUserService userService, IMusicService musicService)
        {
            _logger = logger;
            _userService = userService;
            _musicService = musicService;
            uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

        }

        [Authorize]
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file, string name)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Invalid file");
            }

            Guid id = Guid.NewGuid();
            string filePath = Path.Combine(uploadsFolder, id.ToString());

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if(!IsFileMP3(filePath))
            {
                _musicService.DeleteMusic(filePath);
                return BadRequest("Invalid file");
            }

            User user = _userService.GetUser(User.Identity!.Name!)!;
            Music music = new Music(id, name, file.Length, user);

            try
            {
                _musicService.UploadMusic(music);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }

            return Ok("File uploaded successfully");
        }

        private bool IsFileMP3(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length < 4)
                {
                    return false;
                }

                byte[] header = new byte[3];
                fs.Read(header, 0, 3);

                return header[0] == 0x49 && header[1] == 0x44 && header[2] == 0x33;
            }
        }
    }
}
