using System.Net;
using System.Security.Cryptography;

namespace Connect4Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSignalR();
            services.AddSingleton(s => new MqttTopicBroker(IPAddress.Loopback, 1883));
            services.AddSingleton(s =>
            {
                MqttTopicBroker broker = s.GetRequiredService<MqttTopicBroker>();
                return new MqttTopicClient(broker.IPAddress, broker.Port);
            });
            services.AddSingleton<LoginService>();
            services.AddSingleton<JwtService>((service) => {
                byte[] keyBytes = new byte[32];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(keyBytes);
                }
                string secretKey = Convert.ToBase64String(keyBytes);

                return new JwtService(secretKey);
                });


            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:8080", "http://localhost:8081")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            //app.UseAuthorization();
            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Connect4Hub>("/connect4hub");

            });
        }
    }
}
