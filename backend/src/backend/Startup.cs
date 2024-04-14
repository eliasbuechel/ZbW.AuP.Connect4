using backend.communication;
using backend.database;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace backend
{
    internal class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration mqttConfig = _configuration.GetSection($"{_environment.EnvironmentName}:MqttClient");
            string? mqttIpAddressString = mqttConfig["IpAddress"];
            if (string.IsNullOrEmpty(mqttIpAddressString))
            {
                Debug.Assert(false);
                mqttIpAddressString = "localhost";
            }
            IPAddress mqttIpAddress = IPAddress.Parse(mqttIpAddressString);
            int mqttPort = mqttConfig.GetValue<int>("Port");
            services.AddSingleton(s => new MqttTopicClient(mqttIpAddress, mqttPort));

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
                throw;
            }
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
    }
}