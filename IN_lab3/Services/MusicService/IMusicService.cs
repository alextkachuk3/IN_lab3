using IN_lab3.Models;

namespace IN_lab3.Services.MusicService
{
    public interface IMusicService
    {
        public Music? GetMusic(Guid id);

        public void UploadMusic(Music music);

        public void DeleteMusic(Guid id, User user);

        List<Music>? GetUserMusic(User user);

        List<Music>? GetAllMusic();
    }
}
