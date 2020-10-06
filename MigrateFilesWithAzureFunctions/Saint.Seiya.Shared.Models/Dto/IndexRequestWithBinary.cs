using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class IndexRequestWithBinary : BaseDto
    {
        [Required]
        public string DocumentClassId { get; set; }
        [Required]
        public string DocumentClass { get; set; }
        [Required]
        public string DocumentType { get; set; }
        public JObject Properties { get; set; }
        public string GetCreatedUser() => User;
        public string GetId() => string.Empty;
        public string GetLastModifiedUser() => User;
        public DateTime GetCreated() => DateTime.UtcNow;
        public DateTime GetModified() => GetCreated();
 
    }
}
