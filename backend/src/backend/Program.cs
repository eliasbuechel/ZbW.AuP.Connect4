using backend.database;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost webAppHost = CreateHostBuilder(args).Build();
            webAppHost.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup(builder => new Startup(builder.Configuration));
                    webBuilder.UseUrls("http://localhost:7136");
                });
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(s => new BackendDbContextFacory(new DbContextOptionsBuilder().UseMySQL("Server=127.0.0.1;Port=3307;Database=R4D4_Dev;Uid=EntityFW;Pwd=PwEF54762-@R4D4lul;").Options));

            WebApplication app = builder.Build();

            {
                BackendDbContextFacory dbContextFactory = app.Services.GetRequiredService<BackendDbContextFacory>();
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

            {
                BackendDbContextFacory dbContextFactory = app.Services.GetRequiredService<BackendDbContextFacory>();
                using BackendDbContext context = dbContextFactory.GetDbContext();

                context.Users.Add(new User { Id = 1 });
                context.SaveChanges();

                User[] users = context.Users.ToArray();
                foreach (var user in users)
                    Console.WriteLine($" user<{user.Id}>");
            }

            app.MapGet("/", () => "Hello World!");
            app.Run();
        }
    }
}
