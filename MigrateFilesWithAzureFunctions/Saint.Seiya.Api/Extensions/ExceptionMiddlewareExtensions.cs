using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Saint.Seiya.Api.Infrastructure.Constants;
using System.Net;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ExceptionMiddlewareExtensions
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// ConfigureExceptionHandler
        /// </summary>
        /// <param name="app"></param>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        public static void UseExceptionHandler(this IApplicationBuilder app, ILogger logger, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = ApiConstants.ContentTypes.Json;

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var message = $"Something went wrong: " + (env.IsDevelopment() ? $"{contextFeature.Error}" : "Internal server error");
                        logger.LogError(message); 

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new ProblemDetails()
                        {
                            Detail = message,
                            Status = context.Response.StatusCode,
                            Type = ApiConstants.ProblemDetailsTypes.Status500 
                        })); 
                    }                    
                });
            });
        }
    }
}
