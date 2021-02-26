using alcobot.service.BackgroundServices.Bot;
using alcobot.service.Infrastructure;
using alcobot.service.Services;
using alcobot.service.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace alcobot.service
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(_configuration.GetSection(nameof(AppSettings)));
            services.AddControllers();
            services.AddTransient<IAlcoCounterService, AlcoCounterService>();
            services.AddTransient<IMessageParserService, MessageParserService>();
            services.AddHostedService<BotBackgroundService>();

            services.AddDbContext<AlcoDBContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString(nameof(AlcoDBContext))));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // apply migrations
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<AlcoDBContext>();
                dbcontext.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
