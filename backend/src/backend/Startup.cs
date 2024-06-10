using backend.communication.mqtt;
using backend.Data;
using backend.Infrastructure;
using backend.game;
using backend.services;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using backend.game.entities;
using backend.services.player;
using backend.communication.signalR.frontendApi;
using backend.communication.signalR.opponentRoboterApi;
using backend.utilities;

namespace backend
{
    public class Startup
    {
        public Startup()
        {
            DotNetEnv.Env.Load();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy", builder =>
                {
                    string cors = DotNetEnv.Env.GetString("CORS");
                    builder.WithOrigins(cors)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
            });
            services.AddControllers();


            services.AddSingleton<MQTTNetTopicClient>(services =>
            {
                var borkerUri = DotNetEnv.Env.GetString("MQTT_CLIENT_BROKER_URI");
                var username = DotNetEnv.Env.GetString("MQTT_CLIENT_USERNAME");
                var password = DotNetEnv.Env.GetString("MQTT_CLIENT_PASSWORD");

                return new MQTTNetTopicClient("ws://mqtt.r4d4.work", "ardu", "Pw12ArduR4D4!");
            });
            services.AddSingleton<IRoboterAPI, MQTTRoboterAPI>();

            services.AddDbContext<BackendDbContext>(options =>
            {
                var connectionString = DotNetEnv.Env.GetString("CONNECTIONSTRING");
                options.UseMySQL(connectionString);
            });
            services.AddSingleton<BackendDbContextFacory>(s =>
            {
                var connectionString = DotNetEnv.Env.GetString("CONNECTIONSTRING");
                DbContextOptionsBuilder options = new DbContextOptionsBuilder();
                options.UseMySQL(connectionString);
                return new BackendDbContextFacory(options.Options);
            });

            ConfigureIdentity(services);
            services.AddSignalR();

            services.AddTransient<EmailSettings>( services =>
            {
                EmailSettings settings = new EmailSettings();
                settings.Host = DotNetEnv.Env.GetString("SMTP_HOST");
                settings.Port = DotNetEnv.Env.GetInt("SMTP_PORT");
                settings.Username = DotNetEnv.Env.GetString("SMTP_USERNAME");
                settings.Password = DotNetEnv.Env.GetString("SMTP_PASSWORD");
                settings.FromAddress = DotNetEnv.Env.GetString("SMTP_FROM_ADDRESS");
                return settings;
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddSingleton<GameResultsService>();

            // connection management
            services.AddSingleton<WebPlayerConnectionManager>();
            services.AddSingleton<AlgorythmPlayerConnectionManager>();
            services.AddSingleton<OpponentRoboterPlayerConnectionManager>();
            services.AddTransient<PlayerConnectionService>();
            services.AddSingleton<FrontendCommunicationManager>();
            services.AddSingleton<AlgorythmPlayerCommunicationManager>();

            services.AddSingleton<FrontendApi>();
            services.AddSingleton<OpponentRoboterHubApi>();


            services.AddSingleton<GameManager>();
            services.AddSingleton<GameBoard>();

            services.AddSingleton<Func<Match, Game>>(s => m =>
            {
                GameBoard gameBoard = s.GetRequiredService<GameBoard>();
                return new Game(m, gameBoard);
            });

            services.AddSingleton<RequestHandlerManager<string>>();
            services.AddSingleton<RequestHandlerManager<PlayerIdentity>>();
        }
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1");
                });

                using var scope = app.ApplicationServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Logger.Log(LogCase.ERROR, "Not able to migrate database.", e);
                }
            }

            app.UseRouting();
            app.UseCors("MyCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGroup("/account").MapIdentityApi<PlayerIdentity>();
                endpoints.MapHub<FrontendHub>("/playerHub");
                endpoints.MapHub<OpponentRoboterHub>("/opponentRoboterApi");
            });

            app.ApplicationServices.GetRequiredService<FrontendCommunicationManager>();
            app.ApplicationServices.GetRequiredService<AlgorythmPlayerCommunicationManager>();
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddAuthentication();
                

            services.AddIdentityCore<PlayerIdentity>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;

                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 2;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 100;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<BackendDbContext>();

            services.AddHttpContextAccessor();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Path = "/";
            });

            services.AddIdentityApiEndpoints<PlayerIdentity>();
            
            services.AddSingleton<TimeProvider>(s => TimeProvider.System);

            services.AddScoped<SignInManager<PlayerIdentity>>();
            services.AddScoped<UserManager<PlayerIdentity>>();

            services.AddScoped<Func<UserManager<PlayerIdentity>>>(s => () => s.GetRequiredService<UserManager<PlayerIdentity>>());
        }
    }
}