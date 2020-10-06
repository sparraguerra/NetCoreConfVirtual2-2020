namespace Saint.Seiya.Api.Infrastructure.Constants
{
    public static partial class ApiConstants
    {
        /// <summary>
        /// Swagger constants
        /// </summary>
        public static class Swagger
        {
            /// <summary>
            /// Swagger endpoint
            /// </summary>
            public static string Endpoint => "/api-docs/v1/swagger.json";
            /// <summary>
            /// Swagger Api Name
            /// </summary>
            public static string ApiName => "Saint Seiya Api";
            /// <summary>
            /// Swagger Api Version
            /// </summary>
            public static string ApiVersion => "v1";
            /// <summary>
            /// Swagger route
            /// </summary>
            public static string RouteTemplate => "api-docs/{documentName}/swagger.json";
            /// <summary>
            /// Swagger route prefix
            /// </summary>
            public static string RoutePrefix => "swagger";
        } 
    }
}
