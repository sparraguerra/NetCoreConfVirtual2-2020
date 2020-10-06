
namespace Saint.Seiya.Shared.Models.Config
{
    public class CosmosDBConfig
    {
        public string Endpoint { get; set; }
        public string AuthKey { get; set; }
        public int MaxRetriesOnThrottling { get; set; }
        public int MaxRetryWaitTimeInSeconds { get; set; }
        public string Collection { get; set; }
        public string ProcessesCollection { get; set; }
        public string TracesCollection { get; set; }
        public string Database { get; set; }
        public string AppId { get; set; }
    }
}
