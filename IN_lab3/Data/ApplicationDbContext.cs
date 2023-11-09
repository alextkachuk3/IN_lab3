using IN_lab3.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IN_lab3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        public DbSet<User>? Users { get; set; }

        public DbSet<Music>? Music { get; set; }

    }
}
