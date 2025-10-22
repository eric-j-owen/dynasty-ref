using Microsoft.EntityFrameworkCore;
using Db.Models;

namespace Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerValue> PlayerValues { get; set; }
    }
}
