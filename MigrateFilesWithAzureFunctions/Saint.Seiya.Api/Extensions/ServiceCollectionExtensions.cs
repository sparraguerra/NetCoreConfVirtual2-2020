using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Saint.Seiya.Api.Infrastructure.Constants;
using Saint.Seiya.Api.Infrastructure.Middleware;
using Saint.Seiya.Api.Infrastructure.Swagger;
using Saint.Seiya.Shared.Models.Exceptions;
using Saint.Seiya.Shared.Models.Infrastructure;
using Saint.Seiya.Shared.Models.ProblemDetails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollectionExtensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// AddSwagger
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiConstants.Swagger.ApiVersion, new OpenApiInfo
                {
                    Version = ApiConstants.Swagger.ApiVersion,
                    Title = ApiConstants.Swagger.ApiName,
                    Description = ApiConstants.Swagger.ApiName
                });
                options.EnableAnnotations();
                options.IgnoreObsoleteProperties();
                options.IgnoreObsoleteActions();
                options.DescribeAllParametersInCamelCase();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition(ApiConstants.Bearer, new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Enter the JWT Token. If you do not have it call Azure AD API. Introduce Bearer before the jwt followed by a white space, eg: **Bearer &lt;yourjwttoken&gt;**",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = ApiConstants.Bearer, //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });

                options.OperationFilter<FileOperationFilter>();
                options.OperationFilter<AuthResponsesOperationFilter>();
                options.OperationFilter<DefaultResponseOperationFilter>(); 
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        /// <summary>
        /// AddCors
        /// </summary>
        /// <param name="services"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public static IServiceCollection AddCors(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
                options.AddPolicy(policyName,
                    currentbuilder =>
                    {
                        currentbuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    })
                );

            return services;
        }

        /// <summary>
        /// AddCustocmApiVersioning
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new HeaderApiVersionReader(ApiConstants.ApiVersionHeader);
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            return services;
        }


        /// <summary>
        /// AddCustomAuthentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAdBearerMultiTenant(options => configuration.Bind("AzureAd", options));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ApiConstants.Policies.TokenPolicyName,
                    policy =>
                    {
                        policy.AddRequirements(new AuthorizationTokenRequirement());
                        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    });
            });

            return services;
        }

        /// <summary>
        /// AddRetryPolicies
        /// </summary>
        /// <param name="services"></param>
        /// <param name="policyOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddRetryPolicies(
            this IServiceCollection services,
            PolicyOptions policyOptions)
        {
            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add(
                nameof(PolicyOptions.RetryPolicy),
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(
                        policyOptions.RetryPolicy.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.RetryPolicy.Delay, retryAttempt))));
            policyRegistry.Add(
                nameof(PolicyOptions.CircuitBreakerPolicy),
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .CircuitBreakerAsync(
                        policyOptions.CircuitBreakerPolicy.ExceptionsAllowedBeforeBreaking,
                        policyOptions.CircuitBreakerPolicy.DurationOfBreak));

            return services;
        }

        /// <summary>
        /// AddHttpClient
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TClientOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            TClientOptions options)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new()
        {
            services
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient((serviceProvider, httpClientOptions) =>
                {
                    httpClientOptions.BaseAddress = options.BaseAddress;
                    httpClientOptions.Timeout = options.Timeout;
                    httpClientOptions.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
                    AddBearerTokenToHeaders(serviceProvider, httpClientOptions);
                })
                .ConfigurePrimaryHttpMessageHandler(x => new DefaultHttpClientHandler())
                .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.RetryPolicy))
                .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.CircuitBreakerPolicy));

            return services;
        }

        /// <summary>
        /// AddHttpClient
        /// </summary>
        /// <typeparam name="TClientOptions"></typeparam>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClient<TClientOptions>(
            this IServiceCollection services,
            string name,
            TClientOptions options)
            where TClientOptions : HttpClientOptions, new()
        {
            services
                .AddHttpClient(name)
                .ConfigureHttpClient((serviceProvider, httpClientOptions) =>
                {
                    httpClientOptions.BaseAddress = options.BaseAddress;
                    httpClientOptions.Timeout = options.Timeout;
                    httpClientOptions.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
                    httpClientOptions.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Text.Plain);
                    httpClientOptions.DefaultRequestHeaders.Add("Accept", "text/json");
                    AddBearerTokenToHeaders(serviceProvider, httpClientOptions);
                })
                .ConfigurePrimaryHttpMessageHandler(x => new DefaultHttpClientHandler())
                .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.RetryPolicy))
                .AddPolicyHandlerFromRegistry(nameof(PolicyOptions.CircuitBreakerPolicy));
                
            return services;
        }

        private static void AddBearerTokenToHeaders(IServiceProvider serviceProvider, System.Net.Http.HttpClient httpClientOptions)
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            var bearerToken = httpContextAccessor.HttpContext.Request
                .Headers["Authorization"]
                .FirstOrDefault(h => h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase));

            if (bearerToken != null)
            {
                httpClientOptions.DefaultRequestHeaders.Add("Authorization", bearerToken);
            }
        }

        /// <summary>
        /// AddConfiguration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            return services;
        }


        /// <summary>
        /// AddCustomProblemDetails
        /// </summary>
        /// <param name="services"></param>
        /// <param name="isDevelopment"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services, bool isDevelopment)
        {
            services.AddProblemDetails(setup =>
            {
                setup.IncludeExceptionDetails = (ctx, ex) => isDevelopment;
                setup.Map<NotImplementedException>(ex => new StatusCodeProblemDetails(StatusCodes.Status501NotImplemented));
                setup.Map<SeiyaBaseException>(exception => new SeiyaProblemDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Title = exception.Message,
                    Detail = exception.StackTrace,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = ApiConstants.ProblemDetailsTypes.Status500
                });
                setup.Map<SeiyaConflictException>(exception => new ConflictProblemDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Title = exception.Message,
                    Detail = exception.StackTrace,
                    Status = StatusCodes.Status409Conflict,
                    Type = ApiConstants.ProblemDetailsTypes.Status409
                });
                setup.Map<SeiyaDataAccessException>(exception => new DataAccessProblemDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Title = exception.Message,
                    Detail = exception.StackTrace,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = ApiConstants.ProblemDetailsTypes.Status500
                });
                setup.Map<SeiyaDataNotFoundException>(exception => new DataNotFoundProblemDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Title = exception.Message,
                    Detail = exception.StackTrace,
                    Status = StatusCodes.Status404NotFound,
                    Type = ApiConstants.ProblemDetailsTypes.Status404
                });
                setup.Map<SeiyaValidationException>(exception => new Saint.Seiya.Shared.Models.ProblemDetails.ValidationProblemDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Title = exception.Message,
                    Detail = exception.StackTrace,
                    Status = StatusCodes.Status400BadRequest,
                    Type = ApiConstants.ProblemDetailsTypes.Status400
                });
                setup.Map<SeiyaBaseException>(exception => new BaseProblemDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Title = exception.Message,
                    Detail = exception.StackTrace,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = ApiConstants.ProblemDetailsTypes.Status500
                });
            });
                
            return services;
        }
    }
}
