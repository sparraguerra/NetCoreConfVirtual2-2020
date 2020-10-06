using System.ComponentModel.DataAnnotations;

namespace Saint.Ikki.Fx.Shared.Models
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
