using backend.communication.mqtt;
using backend.communication.signalR;
using backend.Data;
using backend.Infrastructure;
using backend.game;
using backend.services;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using backend.game.entities;


namespace backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
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
            services.AddSingleton<PlayerRequestLock>();
            services.AddScoped<Func<PlayerIdentity, ToPlayerHub<WebPlayerHub>>>(s => {
                GameManager gameManager = s.GetRequiredService<GameManager>();
                IHubContext<WebPlayerHub> hubContext = s.GetRequiredService<IHubContext<WebPlayerHub>>();
                return (identity) => new ToPlayerHub<WebPlayerHub>(identity.Id, identity.UserName == null ? "" : identity.UserName, gameManager, hubContext);
            });

            services.AddSingleton<ToPlayerHub<OpponentRoboterPlayerHub>>(s =>
            {
                GameManager gameManager = s.GetRequiredService<GameManager>();
                IHubContext<OpponentRoboterPlayerHub> hubContext = s.GetRequiredService<IHubContext<OpponentRoboterPlayerHub>>();
                string roboterName = "Opponent roboter";
                string roboterId = Guid.NewGuid().ToString();
                return new ToPlayerHub<OpponentRoboterPlayerHub>(roboterId, roboterName, gameManager, hubContext);
            });

            services.AddSingleton<Func<AlgorythmPlayer>>(s => () =>
            {
                GameManager gameManager = s.GetRequiredService<GameManager>();
                return new AlgorythmPlayer(gameManager);
            });
            services.AddSingleton<AlgorythmPlayerProvider>();

            services.AddSingleton<GameResultsService>();
            services.AddSingleton<IOnlinePlayerProvider>(s => s.GetRequiredService<PlayerConnectionManager>());
            services.AddSingleton<PlayerConnectionManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<Connect4Board>();


            services.AddSingleton<Func<Match, Connect4Game>>(s => m =>
            {
                return new Connect4Game(m, s.GetRequiredService<Connect4Board>());
            });
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
                endpoints.MapHub<WebPlayerHub>("/playerHub");
            });
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

        private readonly IConfiguration _configuration;
    }
}