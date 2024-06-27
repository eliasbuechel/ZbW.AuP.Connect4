using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace backend.data
{
    internal class BackendDesignTimeDbContextFactory : IDesignTimeDbContextFactory<BackendDbContext>
    {
        public BackendDbContext CreateDbContext(string[] args)
        {
            DotNetEnv.Env.Load();
            string connectionString = DotNetEnv.Env.GetString("CONNECTIONSTRING");

            DbContextOptions options = new DbContextOptionsBuilder().UseMySQL(connectionString).Options;
            return new BackendDbContext(options);
        }
    }
}
