using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Saint.Ikki.Fx.ApplicationServices.Abstract.Clients;
using Saint.Ikki.Fx.ApplicationServices.Clients;
using Saint.Ikki.Fx.Common;
using Saint.Ikki.Fx.Shared.Models.Infrastructure;
using Saint.Ikki.Fx.Utils.Constants;
using System.IO;

[assembly: FunctionsStartup(typeof(Saint.Ikki.Fx.Startup))]
namespace Saint.Ikki.Fx
{
    [System.Runtime.InteropServices.Guid("9DF0CEDD-9187-4684-A5FD-A10FA8496A4E")]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();             

            var configuration = config.Build();
            builder.Services.AddOptions();
            builder.Services.AddSingleton<IConfiguration>(configuration);

            builder.Services.AddHttpContextAccessor(); 
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>(); 
            builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings")); 
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ISeiyaApiClient, SeiyaApiClient>(); 
            builder.Services.AddSingleton<Queries>();
            builder.Services.AddSingleton<StatusUpdater>();

            builder.Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            ApplicationLogging.Configuration = configuration;
            ApplicationLogging.ConfigureLogger();
        } 
    }
}

