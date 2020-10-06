using Newtonsoft.Json;

namespace Saint.Seiya.Shared.Models
{
    public class ProcessResponse : Entity
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
