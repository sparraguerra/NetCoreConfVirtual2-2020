using System.IO;

namespace Saint.Seiya.Shared.Models
{
    public class UploadRequestBase
    {
        public Stream FileStream { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public string OriginalFileName { get; set; }
    }
}
