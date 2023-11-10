namespace IN_lab3.Models
{
    public class UserDto
    {
        public UserDto(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
