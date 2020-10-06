namespace Saint.Ikki.Fx.Shared.Models.Seiya
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;

    public class ResponseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        public string DocumentClass { get; set; }

        public string DocumentClassId { get; set; }

        public string DocumentType { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public DateTime Created { get; set; }

        public string CreatedUser { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> Properties { get; set; }
    }
}
