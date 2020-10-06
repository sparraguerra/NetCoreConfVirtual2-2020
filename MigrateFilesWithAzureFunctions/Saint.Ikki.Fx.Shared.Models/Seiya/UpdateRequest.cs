namespace Saint.Ikki.Fx.Shared.Models.Seiya
{ 
    using Newtonsoft.Json.Linq; 
    using System.ComponentModel.DataAnnotations;


    public class UpdateRequest : BaseModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string DocumentClassId { get; set; }
        public string DocumentType { get; set; }
        public JObject Properties { get; set; }
    }
}
