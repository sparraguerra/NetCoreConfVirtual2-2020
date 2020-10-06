using System;

namespace Saint.Seiya.Shared.Models.Infrastructure
{
    public class CircuitBreakerPolicy
    {
        public TimeSpan DurationOfBreak { get; set; }

        public int ExceptionsAllowedBeforeBreaking { get; set; }
    }
}
