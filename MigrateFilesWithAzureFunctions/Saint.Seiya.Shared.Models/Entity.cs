using Newtonsoft.Json;

namespace Saint.Seiya.Shared.Models
{
    public abstract class Entity
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Partitiion Key identifier
        /// </summary>
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
    }
}
