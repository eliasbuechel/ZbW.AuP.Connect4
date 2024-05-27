using backend.communication.mqtt;

namespace backend
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            RoboterMQTTMock roboterMQTTMock = new RoboterMQTTMock();

            IHost webAppHost = CreateHostBuilder(args).Build();
            webAppHost.Run();

            roboterMQTTMock.Dispose();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  DotNetEnv.Env.Load();

                  webBuilder.UseStartup(builder => new Startup());
                  string url = DotNetEnv.Env.GetString("URL");
                  webBuilder.UseUrls(url);
              });
        }
    }
}