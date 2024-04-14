using backend.database;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost webAppHost = CreateHostBuilder(args).Build();

            try
            {
                BackendDbContextFacory dbContextFactory = webAppHost.Services.GetRequiredService<BackendDbContextFacory>();
                using BackendDbContext context = dbContextFactory.GetDbContext();

                context.Users.Add(new User { Id = 1 });
                context.SaveChanges();

                User[] users = context.Users.ToArray();
                foreach (var user in users)
                    Console.WriteLine($" user<{user.Id}>");
            }
            catch (Exception e)
            {
                Logger.Log(LogCase.ERROR, "Not able to run test on database!", e);
            }

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
        }
    }
}
