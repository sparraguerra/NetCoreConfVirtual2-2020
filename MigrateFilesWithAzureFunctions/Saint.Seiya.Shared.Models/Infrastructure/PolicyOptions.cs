namespace Saint.Seiya.Shared.Models.Infrastructure
{
    public class PolicyOptions
    {
        public RetryPolicyOptions RetryPolicy { get; set; }
        public CircuitBreakerPolicy CircuitBreakerPolicy { get; set; }
    }
}
