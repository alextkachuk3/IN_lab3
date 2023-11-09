using IN_lab3.Data;
using IN_lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace IN_lab3.Services.MusicService
{
    public class MusicService : IMusicService
    {
        private readonly ApplicationDbContext _dbContext;

        public MusicService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DeleteMusic(Guid id, User user)
        {
            Music? file = _dbContext.Music!.Where(i => i.Id.Equals(id)).Include(i => i.User).FirstOrDefault();
            if (file is not null)
            {
                if (file.User!.Equals(user))
                {
                    try
                    {
                        _dbContext.Music!.Remove(file);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        _dbContext.SaveChanges();
                        DeleteMusic(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music", id.ToString()));
                    }
                }
                else
                {
                    throw new InvalidOperationException("User tried to delete a music file that doesn't own!");
                }
            }
        }

        public void DeleteMusic(string filePath)
        {
            File.Delete(filePath);
        }

        public List<Music>? GetAllMusic()
        {
            return _dbContext.Music?.Include(i => i.User!.Username).ToList();
        }

        public Music? GetMusic(Guid id)
        {
            return _dbContext.Music?.Where(i => i.Id.Equals(id)).FirstOrDefault();
        }

        public List<Music>? GetUserMusic(User user)
        {
            return _dbContext.Music?.Where(i => i.User!.Equals(user)).ToList();
        }

        public void UploadMusic(Music music)
        {
            try
            {
                _dbContext.Music?.Add(music);
            }
            catch
            {
                throw;
            }
            finally
            {
                _dbContext.SaveChanges();
            }

        }
    }
}
