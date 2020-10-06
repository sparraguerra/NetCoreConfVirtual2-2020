using Saint.Ikki.Fx.Shared.Models.Infrastructure;

namespace Saint.Ikki.Fx.Shared.Models.Seiya.Infraestructure
{
    public class AzureAdOptions
    {  
        public string ClientId { get; set; }        
        public string ClientSecret { get; set; }         
        public string Instance { get; set; }         
        public string Domain { get; set; }         
        public string TenantId { get; set; }
        public string Authority => Instance + TenantId; 
        public string Resource { get; set; }
    }
    public class SeiyaClientApiConfig
    {
        public AzureAdOptions AzureAd { get; set; }
        public HttpClientOptions HttpClientOptions { get; set; } 
        public string  UserName{ get; set; }
        public string DocumentClass { get; set; }
        public string DocumentClassId { get; set; }
        public string DocumentType { get; set; }
    }
}
