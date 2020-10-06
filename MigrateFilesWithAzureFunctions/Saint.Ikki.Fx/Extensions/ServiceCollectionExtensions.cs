using Microsoft.AspNetCore.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Saint.Ikki.Fx.Shared.Models.Infrastructure;
using System;
using System.Linq;
using System.Net.Mime;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollectionExtensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
         

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
    }
}
