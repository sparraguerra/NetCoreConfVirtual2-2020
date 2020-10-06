using Newtonsoft.Json;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class TraceProcessResponse : ProcessResponse
    {
        [JsonProperty(PropertyName = "userMail")]
        public string UserMail { get; set; }
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "processType")]
        public string ProcessType { get; set; }
        [JsonProperty(PropertyName = "uniqueId")]
        public string UniqueId { get; set; }
    }
}
