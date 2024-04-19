using backend.communication;
using backend.database;
using Microsoft.AspNetCore.Identity;
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
            services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            services.AddSwaggerGen();
            services.AddControllers();

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

            string connectionString = _configuration[$"{_environment.EnvironmentName}:ConnectionString"] ?? throw new Exception("Connection string not found in configuration.");
            services.AddSingleton(s => new BackendDbContextFacory(new DbContextOptionsBuilder().UseMySQL(connectionString).Options));
            services.AddScoped<BackendDbContext>(s => s.GetRequiredService<BackendDbContextFacory>().GetDbContext());

            ConfigureIdentity(services);
        }
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors("MyCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapIdentityApi<IdentityUser>();
            });

            BackendDbContextFacory dbContextFactory = app.ApplicationServices.GetRequiredService<BackendDbContextFacory>();
            using BackendDbContext context = dbContextFactory.GetDbContext();
            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Logger.Log(LogCase.ERROR, "Not able to migrate database.", e);
            }
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentityCore<IdentityUser>(options =>
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

            services.AddTransient<IEmailSender<IdentityUser>, EmailSender>();

            services.AddHttpContextAccessor();
            services.AddSingleton<TimeProvider>(s => TimeProvider.System);

            services.AddScoped<SignInManager<IdentityUser>>();
            services.AddScoped<UserManager<IdentityUser>>();

            services.AddScoped<SignInManager<IdentityUser>>();
            services.AddScoped<UserManager<IdentityUser>>();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/login";
                //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
    }
}