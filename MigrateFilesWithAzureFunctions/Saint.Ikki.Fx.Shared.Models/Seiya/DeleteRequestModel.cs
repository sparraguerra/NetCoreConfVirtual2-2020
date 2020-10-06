using System.ComponentModel.DataAnnotations;

namespace Saint.Ikki.Fx.Shared.Models.Seiya
{  
    public class DeleteRequestModel : RequestModel
    {
        [Required]
        public string DocumentClassId { get; set; } 
    }
}
