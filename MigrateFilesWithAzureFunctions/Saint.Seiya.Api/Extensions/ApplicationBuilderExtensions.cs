using Saint.Seiya.Api.Infrastructure.Constants;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// ApplicationBuilderExtensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// UseSwagger
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(options =>
            { 
                options.RouteTemplate = ApiConstants.Swagger.RouteTemplate;
                })
                .UseSwaggerUI(options =>
                {
                    options.DocumentTitle = ApiConstants.Swagger.ApiName;
                    options.SwaggerEndpoint(ApiConstants.Swagger.Endpoint, $"{ApiConstants.Swagger.ApiName} {ApiConstants.Swagger.ApiVersion}");
                    options.RoutePrefix = ApiConstants.Swagger.RoutePrefix;
                    options.DocExpansion(DocExpansion.None);
                });

            return app;
            }
        }
    }
