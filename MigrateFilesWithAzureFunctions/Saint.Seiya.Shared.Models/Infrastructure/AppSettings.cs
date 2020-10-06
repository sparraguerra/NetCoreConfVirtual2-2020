using Saint.Seiya.Shared.Models.Config;

namespace Saint.Seiya.Shared.Models.Infrastructure
{
    public class AppSettings
    {
        public PolicyOptions PolicyConfig { get; set; }
        public SaintIkkiConfig SaintIkkiConfig { get; set; }
    }
}
