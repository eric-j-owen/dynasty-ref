using Microsoft.EntityFrameworkCore;
using Db.Models;
using Shared.Consts;

namespace Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PlayerModel> Players { get; set; }
        public DbSet<PlayerValueModel> PlayerValues { get; set; }
        public DbSet<ExternalIdModel> ExternalIdPlayerLookups { get; set; }
    }
}
