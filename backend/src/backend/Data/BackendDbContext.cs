using backend.Data.entities;
using backend.game;
using backend.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    internal class BackendDbContext : IdentityDbContext<PlayerIdentity>
    {
        public BackendDbContext(DbContextOptions options) : base(options) { }

        public DbSet<DbGameResult> GameResults { get; set; }
        public DbSet<DbGameResultMatch> Matches { get; set; }
        public DbSet<DbPlayerInfo> Players { get; set; }
        public DbSet<DbField> Fields { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => Logger.Log(LogCase.DEBUG, $"ENTITY_FRAMEWORK: {message}"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}