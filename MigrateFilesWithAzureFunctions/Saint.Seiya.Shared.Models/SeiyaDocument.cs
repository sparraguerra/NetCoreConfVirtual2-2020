using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Saint.Seiya.Shared.Models
{
    public class SeiyaDocResponse : Base
    {
        public string DocumentClass { get; set; }
        public string DocumentClassId { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public bool IsDeleted { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string BlobName { get; set; }
        [JsonExtensionData]
        public IDictionary<string, JToken> Properties { get; set; }
        public string GetDocumentId() => Id;
        public string GetContentType() => string.Empty;
    }

    public class SeiyaDocument : SeiyaDocResponse
    {        
        public string Container { get; set; }
        public string FolderName { get; set; }

        public void Delete(string user)
        {
            LastModifiedUser = user;
            Modified = DateTime.UtcNow;
            IsDeleted = true; 
        }
        public void Restore(string user)
        {
            LastModifiedUser = user;
            Modified = DateTime.UtcNow;
            IsDeleted = false; 
        }
    }
}
