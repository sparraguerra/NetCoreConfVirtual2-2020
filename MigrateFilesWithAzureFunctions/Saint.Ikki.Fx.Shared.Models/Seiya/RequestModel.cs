namespace Saint.Ikki.Fx.Shared.Models.Seiya
{
    using System.ComponentModel.DataAnnotations;

    public class RequestModel : BaseModel
    {
        [Required]
        public string Id { get; set; }
    }
}
