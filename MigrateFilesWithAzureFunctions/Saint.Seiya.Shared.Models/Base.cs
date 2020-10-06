using System;

namespace Saint.Seiya.Shared.Models
{
    public abstract class Base : Entity
    {  
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string CreatedUser { get; set; }
        public string LastModifiedUser { get; set; }

        public string AbsoluteUri { get; set; }
        public Uri GetUri()
        {
            return new Uri(this.AbsoluteUri);
        }
    }
}
