using alcobot.service.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace alcobot.service.tests
{
    public class OptionsFixture
    {
        public ServiceProvider ServiceProvider { get; }
        public OptionsFixture()
        {
            IServiceCollection services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddOptions();
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
            services.AddLogging(builder => { builder.AddDebug(); builder.SetMinimumLevel(LogLevel.Trace); });

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
