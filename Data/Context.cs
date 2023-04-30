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
        public DbSet<Event> Events{ get; set; }
        public DbSet<SubEvent> SubEvents { get; set; }
        public DbSet<SubEventType> SubEventTypes { get; set; }
        // Add your DbSet properties here
    }
}
