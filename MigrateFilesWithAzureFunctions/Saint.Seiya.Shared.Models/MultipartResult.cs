using Saint.Seiya.Shared.Models.Dto;

namespace Saint.Seiya.Shared.Models
{
    public class MultipartResult<T> where T : BaseDto
    {
        public UploadRequest UploadRequest { get; set; }
        public T Document { get; set; }
    }
}
