using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// AzureAdOptions
    /// </summary>
    public class AzureAdOptions
    {
        /// <summary>
        /// Cosntructor
        /// </summary>
        public AzureAdOptions(){ }

        /// <summary>
        /// ClientId
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// ClientSecret
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Instance
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Domain
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// TenantId
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// CallbackPath
        /// </summary>
        public string CallbackPath { get; set; }
    }

    /// <summary>
    /// AzureAdServiceCollectionExtensions
    /// </summary>
    public static class AzureAdServiceCollectionExtensions
    {
        /// <summary>
        /// AddAzureAdBearer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder) => builder.AddAzureAdBearer(_ => { });

        /// <summary>
        /// AddAzureAdBearer
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAzureAdBearer(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
            builder.AddJwtBearer();
            return builder;
        }

        /// <summary>
        /// AddAzureAdBearerMultiTenant
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAzureAdBearerMultiTenant(this AuthenticationBuilder builder) => builder.AddAzureAdBearer(_ => { });

        /// <summary>
        /// AddAzureAdBearerMultiTenant
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddAzureAdBearerMultiTenant(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureAzureOptions>();
            builder.AddJwtBearer(j =>
            {
                j.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            return builder;
        }

        private class ConfigureAzureOptions : IConfigureNamedOptions<JwtBearerOptions>
        {
            private readonly AzureAdOptions azureOptions;

#pragma warning disable S1144 // Unused private types or members should be removed
            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
            {
                this.azureOptions = azureOptions.Value;
            }
#pragma warning restore S1144 // Unused private types or members should be removed

            public void Configure(string name, JwtBearerOptions options)
            {
                options.Audience = this.azureOptions.ClientId;
                options.Authority = $"{this.azureOptions.Instance}{this.azureOptions.TenantId}";
                options.SaveToken = true;                
            }

            public void Configure(JwtBearerOptions options)
            {
                Configure(Options.Options.DefaultName, options);
            }
        }
    }
}
