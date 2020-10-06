using Newtonsoft.Json;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class LaunchProcessRequest : BaseDto
    {
        [JsonProperty(PropertyName = "processType")]
        public int ProcessType { get; set; }
    }
}
