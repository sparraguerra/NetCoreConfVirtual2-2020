using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Saint.Seiya.Api.Infrastructure.Swagger
{
    /// <summary>
    /// FileOperationFilter
    /// </summary>
    public class FileOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody == null || !operation.RequestBody.Content.ContainsKey("multipart/form-data"))
            {
                return;
            }

            operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Clear();
            operation.RequestBody.Content["multipart/form-data"].Encoding.Clear();

            var parameters = context.ApiDescription.ActionDescriptor.Parameters.Where(p => p.BindingInfo.BindingSource.Id == "Form");

            foreach (var parameter in parameters)
            {
                var properties = parameter.ParameterType.GetProperties();
                foreach (var property in properties)
                {
                    var argumentName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                    SetOpenApiOperationSchema(operation, property, argumentName);
                }
            }
        }

        private void SetOpenApiOperationSchema(OpenApiOperation operation, PropertyInfo property, string argumentName)
        {
            if (property.PropertyType == typeof(Stream) || property.PropertyType == typeof(IFormFile))
            {
                operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(
                    argumentName,
                    new OpenApiSchema() { Type = "string", Format = "binary" }
                );
            }
            else
            {
                if (!argumentName.ToLowerInvariant().Contains("file"))
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(
                        argumentName,
                        new OpenApiSchema() { Type = "string", Format = "text" }
                    );
                }
            }
        }
    }
}
