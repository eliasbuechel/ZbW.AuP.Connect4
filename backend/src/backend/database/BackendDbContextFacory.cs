using Microsoft.EntityFrameworkCore;

namespace backend.database
{
    internal class BackendDbContextFacory
    {
        public BackendDbContextFacory(DbContextOptions options)
        {
            _options = options;
        }

        public BackendDbContext GetDbContext()
        {
            return new BackendDbContext(_options);
        }

        private readonly DbContextOptions _options;
    }
}
