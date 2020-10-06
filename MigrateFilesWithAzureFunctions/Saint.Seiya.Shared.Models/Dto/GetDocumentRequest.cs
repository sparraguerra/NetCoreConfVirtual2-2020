using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class GetDocumentRequest : BaseDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string DocumentClassId { get; set; }
    }
}
