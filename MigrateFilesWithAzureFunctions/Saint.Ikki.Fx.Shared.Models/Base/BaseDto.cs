using System.ComponentModel.DataAnnotations;

namespace Saint.Ikki.Fx.Shared.Models.Base
{
    public abstract class BaseDto
    {
        [Required]
        public string User { get; set; }         
    }
}
