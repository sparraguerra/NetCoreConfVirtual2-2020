using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class UpdateRequest : BaseDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string DocumentClassId { get; set; }
        public string DocumentType { get; set; }
        public JObject Properties { get; set; } 
    }
}
