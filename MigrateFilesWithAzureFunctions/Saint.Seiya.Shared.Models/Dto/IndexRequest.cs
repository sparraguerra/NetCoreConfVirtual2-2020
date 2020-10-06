using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class IndexRequest : BaseDto
    {
        [Required]
        public string DocumentClassId { get; set; }
        [Required]
        public string DocumentClass { get; set; }
        [Required]
        public string DocumentType { get; set; }
        [Required]
        public string FileName { get; set; }
        public JObject Properties { get; set; }
        public string GetCreatedUser() => User;
        public string GetId() => string.Empty;
        public Uri GetUri() => null;
        public string GetLastModifiedUser() => User;
        public DateTime GetCreated() => DateTime.UtcNow;
        public DateTime GetModified() => GetCreated();
        public string GetContentType() => string.Empty;
#pragma warning disable S3400 // Methods should not return constants. We use it for automapper config
        public long GetSize() => 0;
        public bool GetIsDeleted() => false;
        public bool GetHasLegalHold() => false;
#pragma warning restore S3400 // Methods should not return constants. We use it for automapper config
    }
}
