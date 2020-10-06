using Microsoft.AspNetCore.Mvc;
using Saint.Seiya.Api.Infrastructure.Constants;
using System.Net;

namespace Saint.Seiya.Api.Api.Extensions
{
    /// <summary>
    /// ActionContextExtensions
    /// </summary>
    public static class ActionContextExtensions
    {
        /// <summary>
        /// ValidationError
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static BadRequestObjectResult ValidationError(this ActionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Instance = context.HttpContext.Request.Path,
                Status = (int)HttpStatusCode.BadRequest,
                Detail = ApiConstants.Messages.ModelStateValidation
            };
            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { ApiConstants.ContentTypes.ProblemJson, ApiConstants.ContentTypes.ProblemXml }
            };
        }
    }
}
