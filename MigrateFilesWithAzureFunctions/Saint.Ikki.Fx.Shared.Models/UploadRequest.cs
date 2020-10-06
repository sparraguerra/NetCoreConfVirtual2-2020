using Saint.Ikki.Fx.Shared.Models.Base;

namespace Saint.Ikki.Fx.Shared.Models
{
    public class UploadRequest : UploadRequestBase
    {
        public string DocumentClass { get; set; }
        public string DocumentClassId { get; set; } 
    }
}
