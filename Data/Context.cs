using mapsProjAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace mapsProjAPI.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<User> Users { get; set; }
        // Add your DbSet properties here
    }
}
