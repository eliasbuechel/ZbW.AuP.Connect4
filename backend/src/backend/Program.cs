using backend.communication;
using System.Diagnostics;
using System.Net;

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
                    webBuilder.UseStartup(builder => new Startup(builder.Configuration));
                    webBuilder.UseUrls("http://localhost:7136");
                });
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");

            ConfigurationManager config = builder.Configuration;
            string environment = builder.Environment.EnvironmentName;


            IConfiguration mqttConfig = config.GetSection($"{environment}:MqttClient");
            string? mqttIpAddressString = mqttConfig["IpAddress"];
            if (string.IsNullOrEmpty(mqttIpAddressString))
            {
                Debug.Assert(false);
                mqttIpAddressString = "localhost";
            }
            IPAddress mqttIpAddress = IPAddress.Parse(mqttIpAddressString);
            int mqttPort = mqttConfig.GetValue<int>("Port");
            builder.Services.AddSingleton(s => new MqttTopicClient(mqttIpAddress, mqttPort));


            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
