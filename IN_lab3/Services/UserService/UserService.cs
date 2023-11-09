using IN_lab3.Data;
using IN_lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace IN_lab3.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddUser(string username, string password)
        {            
            try
            {
                _dbContext.Users?.Add(new User(username, password));
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

        public User? GetUser(string username)
        {
            return _dbContext.Users?.FirstOrDefault(u => u.Username!.Equals(username));
        }

        public bool IsUserNameUsed(string username)
        {
            return _dbContext.Users?.Where(u => u.Username!.Equals(username)).Count() > 0;
        }
    }
}
