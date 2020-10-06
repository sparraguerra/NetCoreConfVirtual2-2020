using System;

namespace Saint.Seiya.Shared.Models.Infrastructure
{
    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
