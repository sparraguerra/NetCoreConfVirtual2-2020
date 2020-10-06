namespace Saint.Seiya.Shared.Models.Config
{
    public class BlobStorageConfig
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
        public string Temp { get; set; }
        public int MinutesSasExpire { get; set; }
    }
}
