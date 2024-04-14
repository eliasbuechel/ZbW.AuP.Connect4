namespace backend
{
    public static class Program
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
                    webBuilder.UseStartup(builder => new Startup(builder.Configuration, builder.HostingEnvironment));
                    webBuilder.UseUrls("http://localhost:7136");
                });
        }
    }
}