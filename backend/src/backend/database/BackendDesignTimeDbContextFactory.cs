using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace backend.database
{
    internal class BackendDesignTimeDbContextFactory : IDesignTimeDbContextFactory<BackendDbContext>
    {
        public BackendDbContext CreateDbContext(string[] args)
        {
            const string connectionString = "Server=127.0.0.1;Port=3306;Database=r4d4_loc;Uid=root;Pwd=root;";

            DbContextOptions options = new DbContextOptionsBuilder().UseMySQL(connectionString).Options;
            return new BackendDbContext(options);
        }
    }
}
