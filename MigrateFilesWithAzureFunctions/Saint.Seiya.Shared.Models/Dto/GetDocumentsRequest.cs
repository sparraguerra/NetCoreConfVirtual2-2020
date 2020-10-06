using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saint.Seiya.Shared.Models.Dto
{
    public class GetDocumentsRequest : BaseDto
    {
        [Required]
        public string DocumentClassName { get; set; }
        [Required]
        public string DocumentClassId { get; set; }

        public IEnumerable<string> FieldsToRetrieve { get; set; }
    }
}
