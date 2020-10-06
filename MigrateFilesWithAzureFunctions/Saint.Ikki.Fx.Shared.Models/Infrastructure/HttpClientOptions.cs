using System;

namespace Saint.Ikki.Fx.Shared.Models.Infrastructure
{
    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
