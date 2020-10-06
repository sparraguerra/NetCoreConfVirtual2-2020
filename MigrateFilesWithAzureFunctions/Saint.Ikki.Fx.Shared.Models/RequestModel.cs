using System.Collections.Generic;

namespace Saint.Ikki.Fx.Shared.Models
{
    public class RequestModel
    {
        public RequestModel()
        {
            Items = new List<SPItem>();
        }

        public string User { get; set; }
        public int Type { get; set; }
        public string UniqueId { get; set; }
        public List<SPItem> Items { get; set; }
    }
}
