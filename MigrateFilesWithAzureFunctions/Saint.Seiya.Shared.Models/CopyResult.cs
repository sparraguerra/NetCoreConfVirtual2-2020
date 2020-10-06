using System;

namespace Saint.Seiya.Shared.Models
{
    public class CopyResult: UploadResult
    {
        public Uri Uri { get; set; }
        public string ContentType { get; set; }
        public string OriginalFileName { get; set; }
        public long Size { get; set; }
    }
}
