using Saint.Seiya.Shared.Models.Enums;

namespace Saint.Seiya.Shared.Models
{
    public class TracesRequest
    {
        public int Page { get; set; }
        public string OrderBy { get; set; }
        public bool Descending { get; set; }
        public ProcessStatus? Filter { get; set; }
        public int ElementsPerPage { get; set; }
    }
}
