using System;

namespace Saint.Ikki.Fx.Shared.Models.Seiya.Metadatas
{
    public class MetadataDocument 
    {
        public string Name { get; set; }
        public string Title { get; set; } 
        public string FileType { get; set; } 
        public string DocumentId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string SessionId { get; set; }
    }
}