namespace Saint.Ikki.Fx.Shared.Models.Infrastructure
{
    public class PolicyOptions
    {
        public RetryPolicyOptions RetryPolicy { get; set; }
        public CircuitBreakerPolicy CircuitBreakerPolicy { get; set; }
    }
}
