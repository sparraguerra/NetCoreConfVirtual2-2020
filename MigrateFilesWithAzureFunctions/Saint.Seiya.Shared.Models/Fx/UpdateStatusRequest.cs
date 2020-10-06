using Saint.Seiya.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models
{
    public class UpdateStatusRequest
    {
        [Required]
        public int ProcessType { get; set; }
        [Required]
        public ProcessStatus Status { get; set; }
        [Required]
        public string UniqueId { get; set; }
    }
}
