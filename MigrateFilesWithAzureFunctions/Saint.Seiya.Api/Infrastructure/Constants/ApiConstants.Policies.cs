namespace Saint.Seiya.Api.Infrastructure.Constants
{
    public static partial class ApiConstants
    {
        /// <summary>
        /// Authorization policies constants
        /// </summary>
        public static class Policies
        {
            /// <summary>
            /// Authorization policy name
            /// </summary>
            public static string TokenPolicyName => "TokenPolicy";

            /// <summary>
            /// Admin Authorization policy name
            /// </summary>
            public static readonly string AdminPolicyName = "AdminPolicy";
        } 
    }
}
