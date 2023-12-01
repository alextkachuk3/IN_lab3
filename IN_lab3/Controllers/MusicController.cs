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

        [HttpGet("{id}")]
        public IActionResult GetMusicFile(string id)
        {
            Music? music = _musicService.GetMusic(Guid.Parse(id));

            if (music == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(uploadsFolder, music.Id.ToString());

            if (System.IO.File.Exists(filePath))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, "audio/mpeg");
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet]
        public IEnumerable<MusicDto> GetUserMusic()
        {
            User user = _userService.GetUser(User.Identity!.Name!)!;
            List<Music> music = _musicService.GetUserMusic(user)!;
            return music.Select(music => new MusicDto(music.Id, music.Name!)).ToArray();
        }

        [Authorize]
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm]string? name)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest(new { Error = "invalid_file" });
            }

            if (name == null || name.Length < 1 || name.Length > 70)
            {
                return BadRequest(new { Error = "invalid_name" });
            }

            Guid id = Guid.NewGuid();
            string filePath = Path.Combine(uploadsFolder, id.ToString());

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!IsFileMP3(filePath))
            {
                _musicService.DeleteMusic(filePath);
                return BadRequest(new { Error = "invalid_file" });
            }

            User user = _userService.GetUser(User.Identity!.Name!)!;
            Music music = new Music(id, name, file.Length, user);

            try
            {
                _musicService.UploadMusic(music);
            }
            catch
            {
                return StatusCode(500, new { Error = "internal_server_error" });
            }

            return Ok(new { Message = "file_uploaded_successfully" });
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
