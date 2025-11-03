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

        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerValue> PlayerValues { get; set; }
        public DbSet<ExternalIdPlayerLookup> ExternalIdPlayerLookups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<DataSource>();
            modelBuilder.HasPostgresEnum<TeamAbbr>();
            modelBuilder.HasPostgresEnum<IncludedPosition>();
        }
    }
}
