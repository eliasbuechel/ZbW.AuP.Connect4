using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options => builder.Configuration.Bind("JwtSettings", options))
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options => builder.Configuration.Bind("CookieSettings", options));

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
