using IN_lab3.Models;

namespace IN_lab3.Services.UserService
{
    public interface IUserService
    {
        public bool IsUserNameUsed(string username);

        public void AddUser(string username, string password);

        public User? GetUser(string username);
    }
}
