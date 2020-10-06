using Saint.Ikki.Fx.Shared.Models.Seiya.Infraestructure;

namespace Saint.Ikki.Fx.Shared.Models.Infrastructure
{
    public class AppSettings
    {
        public PolicyOptions PolicyConfig { get; set; }  
        public SeiyaClientApiConfig SeiyaApiClientConfig { get; set; }
        public SharepointConfig SharepointConfig { get; set; }
    }
}
