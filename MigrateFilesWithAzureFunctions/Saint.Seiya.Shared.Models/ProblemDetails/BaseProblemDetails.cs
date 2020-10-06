namespace Saint.Seiya.Shared.Models.ProblemDetails
{
    public class BaseProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public string ErrorCode { get; set; }
    }
}
