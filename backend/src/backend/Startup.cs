using backend.database;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(s => new BackendDbContextFacory(new DbContextOptionsBuilder().UseMySQL("Server=127.0.0.1;Port=3307;Database=R4D4_Dev;Uid=EntityFW;Pwd=PwEF54762-@R4D4lul;").Options));
        }
        public void Configure(IApplicationBuilder app)
        {
            BackendDbContextFacory dbContextFactory = app.ApplicationServices.GetRequiredService<BackendDbContextFacory>();
            using BackendDbContext context = dbContextFactory.GetDbContext();
            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Logger.Log(LogCase.ERROR, "Not able to migrate database", e);
                return;
            }
        }
    }
}