using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace NN.MigracionHistoricos.Api.Infrastructure.Conventions
{
    /// <summary>
    /// SlugifyParameterTransformer
    /// </summary>
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        /// TransformOutbound
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string TransformOutbound(object value)
        {
            if (value == null)
            {
                return null;
            }

            // Slugify value
            return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        }
    }
}
