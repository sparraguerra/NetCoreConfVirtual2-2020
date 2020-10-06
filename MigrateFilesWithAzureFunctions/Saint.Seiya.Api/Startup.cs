using Autofac;
using AutoMapper;
using Encamina.Gada.Domain.Infrastructure.Automapper;
using Hellang.Middleware.ProblemDetails;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NN.MigracionHistoricos.Api.Infrastructure.Conventions;
using Saint.Seiya.Domain.Infrastructure.AutofacModules;
using Saint.Seiya.Infrastructure.Infrastructure.AutofacModules;
using Saint.Seiya.Services.Infrastructure.AutofacModules;
using Saint.Seiya.Shared.Models.Config;
using Saint.Seiya.Shared.Models.Infrastructure;
using Serilog;
using System.Diagnostics;

namespace Saint.Seiya.Api
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Environment
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddOptions()
                .AddApplicationInsightsTelemetry(options =>
                {
                    options.DependencyCollectionOptions.EnableLegacyCorrelationHeadersInjection = true;
                    options.InstrumentationKey = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
                    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
                    Activity.ForceDefaultIdFormat = true;
                })
                .AddCors("AllowAllOriginsPolicy")
                .AddCustomAuthorization(Configuration)
                .AddConfiguration(Configuration)
                .AddCustomProblemDetails(Environment.IsDevelopment())
                .AddCustomApiVersioning()
                .AddSwagger();
            
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            })
            .AddJsonSerializerSettings()
            .AddApiBehaviorOptions();
             
            services.Configure<CosmosDBConfig>(Configuration.GetSection("CosmosDb"));
            services.Configure<BlobStorageConfig>(Configuration.GetSection("BlobStorage"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddSingleton(Configuration);
            services.AddAutoMapper(typeof(ServiceProfile));
            services.AddHttpClient();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAllOriginsPolicy");
            app.UseSwagger();
            app
            .UseProblemDetails()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseExceptionHandler(loggerFactory.CreateLogger<ProblemDetails>(), env);
            ConfigureLogger(loggerFactory);
        }

        /// <summary>
        /// ConfigureContainer
        /// </summary>
        /// <param name="builder"></param>
        public static void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(new LoggerFactory()).As<ILoggerFactory>();

            builder.RegisterGeneric(typeof(Logger<>))
                   .As(typeof(ILogger<>))
                   .SingleInstance();

            builder.RegisterModule<InfrastructureModules>();
            builder.RegisterModule<DomainModules>();
            builder.RegisterModule<ServiceModules>(); 
        }

        /// <summary>
        /// Configure logger factory
        /// </summary>
        /// <param name="loggerFactory"></param>
        private void ConfigureLogger(ILoggerFactory loggerFactory)
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .Enrich.FromLogContext()
                            .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Events)
                            .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                            .WriteTo.Console()
                            .CreateLogger();

            loggerFactory.AddSerilog(Log.Logger);
        }
    }
}
