using backend.communication;
using System.Diagnostics;
using System.Net;

namespace backend
{
    public static class Program
    {
        public static void Main(string[] args)
        {
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
