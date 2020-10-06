using System.IO;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class GetFileResponse
    {
        public Stream File { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
