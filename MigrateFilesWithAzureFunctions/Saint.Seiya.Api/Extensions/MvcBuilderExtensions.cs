using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Saint.Seiya.Api.Infrastructure.Constants;
using Saint.Seiya.Api.Infrastructure.Conventions;
using System.Globalization;
using System.Net;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// MvcBuilderExtensions
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// AddApiBehaviorOptions
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddApiBehaviorOptions(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = (int)HttpStatusCode.BadRequest,
                        Type = "about:blank",
                        Detail = ApiConstants.Messages.ModelStateValidation
                    };
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = 
                        { 
                            ApiConstants.ContentTypes.ProblemJson, 
                            ApiConstants.ContentTypes.ProblemXml
                        }
                    };
                };
            });

            return builder;
        }

        /// <summary>
        /// AddJsonSerializerSettings
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddJsonSerializerSettings(this IMvcBuilder builder)
        {
            builder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new IgnoreEmptyEnumerablesResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Culture = CultureInfo.GetCultureInfo("es-ES");
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.UseCamelCasing(true);
            });

            return builder;
        }
    }
}
