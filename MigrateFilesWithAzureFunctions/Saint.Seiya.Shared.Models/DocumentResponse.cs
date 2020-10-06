using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Saint.Seiya.Shared.Models
{
    public class DocumentResponse : Base
    {
        public string DocumentClass { get; set; }
        public string DocumentClassId { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }       
        public string Container { get; set; }
        public string FolderName { get; set; }
        public string BlobName { get; set; }
        [JsonExtensionData]
        public IDictionary<string, JToken> Properties { get; set; }
    }
}
