﻿namespace Saint.Seiya.Shared.Models.Infrastructure
{
    public class RetryPolicyOptions
    {
        public int Count { get; set; }

        public int Delay { get; set; }
    }
}
