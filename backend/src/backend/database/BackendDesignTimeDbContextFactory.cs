using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace backend.database
{
    internal class BackendDesignTimeDbContextFactory : IDesignTimeDbContextFactory<BackendDbContext>
    {
        public BackendDbContext CreateDbContext(string[] args)
        {
            const string connectionString = "Server=127.0.0.1;Port=3307;Database=R4D4_Dev;Uid=EntityFW;Pwd=PwEF54762-@R4D4lul;";

            DbContextOptions options = new DbContextOptionsBuilder().UseMySQL(connectionString).Options;
            return new BackendDbContext(options);
        }
    }
}
