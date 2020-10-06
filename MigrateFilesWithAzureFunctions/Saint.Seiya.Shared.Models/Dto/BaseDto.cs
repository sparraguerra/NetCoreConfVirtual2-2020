using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class BaseDto
    {
        [Required]
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
    }
}
