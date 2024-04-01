namespace backend
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) 
        {
            return; 
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
