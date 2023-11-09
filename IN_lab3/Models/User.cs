using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace IN_lab3.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public User() { }

        public User(string username, string password)
        {
            Username = username;
            Salt = GenerateSalt();
            PasswordHash = HashPassword(password, Salt);
        }

        public bool CheckCredentials(string password, byte[] salt)
        {
            return PasswordHash!.SequenceEqual(HashPassword(password, salt));
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(32)]
        public byte[]? PasswordHash { get; set; }

        [Required]
        [MaxLength(16)]
        public byte[]? Salt { get; set; }

        private static Regex regex = new Regex("^[a-zA-Z0-9]*$");

        public static bool IsAlphanumeric(string input)
        {
            return regex.IsMatch(input);
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private byte[] HashPassword(string password, byte[] salt)
        {
            byte[] hashedPassword = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            return hashedPassword;
        }
    }
}
