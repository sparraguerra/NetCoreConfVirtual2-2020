using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class ModelFunctionRequest
    {
        [Required]
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
        [Required]
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }
        [Required]
        [JsonProperty(PropertyName = "uniqueId")]
        public string UniqueId { get; set; }
    }
}
