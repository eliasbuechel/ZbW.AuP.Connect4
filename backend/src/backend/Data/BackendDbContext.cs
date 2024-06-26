﻿using backend.data.entities;
using backend.infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.data
{
    internal class BackendDbContext : IdentityDbContext<PlayerIdentity>
    {
        public BackendDbContext(DbContextOptions options) : base(options) { }

        public DbSet<DbGameResult> GameResults { get; set; }
        public DbSet<DbGameResultMatch> Matches { get; set; }
        public DbSet<DbPlayerInfo> Players { get; set; }
        public DbSet<DbField> Fields { get; set; }
        public DbSet<DbPlayedMove> PlayerMoves { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => Logger.Log(LogLevel.Debug, LogContext.ENTITY_FRAMEWORK, message));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}