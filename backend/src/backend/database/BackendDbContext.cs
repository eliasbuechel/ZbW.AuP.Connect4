using Microsoft.EntityFrameworkCore;

namespace backend.database
{
    internal class BackendDbContext : DbContext
    {
        public BackendDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => Logger.Log(LogCase.DEBUG, $"ENTITY_FRAMEWORK: {message}"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
