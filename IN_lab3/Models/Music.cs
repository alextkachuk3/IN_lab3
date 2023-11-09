using IN_lab3.Models;
using System.ComponentModel.DataAnnotations;

namespace IN_lab3.Models
{

    public class Music
    {
        public Music() { }

        public Music(Guid id, string name, long fileSize, User user)
        {
            Id = id;
            Name = name;
            FileSize = fileSize;
            User = user;
            UploadDate = DateTime.Now;
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 3)]
        public string? Name { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public User? User { get; set; }
    }
}
