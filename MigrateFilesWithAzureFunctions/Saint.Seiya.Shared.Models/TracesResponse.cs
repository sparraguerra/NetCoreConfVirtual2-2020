using Saint.Seiya.Shared.Models.Dto;
using System.Collections.Generic;

namespace Saint.Seiya.Shared.Models
{
    public class TracesResponse
    {
        public IEnumerable<TraceProcessResponse> ProcessList { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
    }
}
