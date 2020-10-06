using System;
using System.Linq;
using System.Reflection;

namespace Saint.Ikki.Fx.Shared.Models.Seiya.Extensions
{    
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetMetadataProperties(this Type type)
        {
            return type
                .GetProperties()
                .Where(pi => Attribute.IsDefined(pi, typeof(MetadataPropertyAttribute)))
                .ToArray();
        }
    }
}
