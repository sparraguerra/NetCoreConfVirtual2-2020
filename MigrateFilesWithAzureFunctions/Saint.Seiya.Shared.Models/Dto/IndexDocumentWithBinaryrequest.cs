using Microsoft.AspNetCore.Http;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class IndexDocumentWithBinaryrequest
    {
        public string Metadata { get; set; }
        public IFormFile File { get; set; }
    }
}
