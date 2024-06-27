namespace backend
{
    internal static class Program
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
                  DotNetEnv.Env.Load();

                  webBuilder.UseStartup<Startup>();
                  string url = DotNetEnv.Env.GetString("URL");
                  webBuilder.UseUrls(url);
              });
        }
    }
}