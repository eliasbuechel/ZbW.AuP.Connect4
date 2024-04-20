using backend.game;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.database
{
    internal class BackendDbContext : IdentityDbContext<PlayerIdentity>
    {
        public BackendDbContext(DbContextOptions options) : base(options)
        { }

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

    internal class PlayerIdentity : IdentityUser
    {
        public Guid PlayerId { get; }
    }
}